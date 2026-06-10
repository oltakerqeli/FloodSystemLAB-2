import { useState, useEffect } from "react";
import { useAuth } from "../../contexts/AuthContext";
import { API_BASE_URL } from "../../utils/apiConfig";
import notificationService from "../../services/notificationService";
import "./NotificationBell.css";

export default function NotificationBell() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const [isConnected, setIsConnected] = useState(false);

  const isAdminOrAuthority = user?.roles?.includes("Admin") || user?.roles?.includes("Authority");
  const unreadCount = notifications.filter(n => !n.read).length;

  const addNotification = (notification) => {
    setNotifications(prev => [{ ...notification, read: false, id: `live_${Date.now()}` }, ...prev]);

    if ("Notification" in window && Notification.permission === "granted") {
      new Notification(notification.title, {
        body: notification.message,
        icon: "🔔"
      });
    }
  };

  const loadNotifications = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/Notifications`, { credentials: "include" });
      if (!res.ok) return;
      const data = await res.json();
      setNotifications(data.map(n => ({
        id: `db_${n.id}`,
        dbId: n.id,
        title: n.type === "report" ? `📋 ${n.title}` : `🔄 ${n.title}`,
        message: n.message,
        type: n.type === "report" ? "report" : "alert",
        read: n.isRead,
        timestamp: new Date(n.createdAt)
      })));
    } catch (err) {
      console.error("Failed to load notifications:", err);
    }
  };

  useEffect(() => {
    if (!user) return;

    if ("Notification" in window && Notification.permission !== "denied") {
      Notification.requestPermission();
    }

    loadNotifications();

    const initSignalR = async () => {
      try {
        await notificationService.connect({
          userId: user.id,
          isAdmin: isAdminOrAuthority
        });
        setIsConnected(true);
      } catch (err) {
        console.error("SignalR setup failed:", err);
      }
    };

    initSignalR();

    notificationService.on("NewReport", (report) => {
      addNotification({
        title: `📋 New ${report.type} Report`,
        message: `${report.location} - ${report.severity}`,
        type: "report",
        timestamp: new Date(report.timestamp)
      });
    });

    notificationService.on("StatusChanged", (data) => {
      addNotification({
        title: `🔄 ${data.reportType} Report #${data.reportId}`,
        message: data.message,
        type: "alert",
        timestamp: new Date(data.timestamp)
      });
    });

    return () => {
      notificationService.off("NewReport");
      notificationService.off("StatusChanged");
      notificationService.disconnect();
    };
  }, [user?.id]);

  const markAsRead = async (notif) => {
    setNotifications(prev => prev.map(n => n.id === notif.id ? { ...n, read: true } : n));
    if (notif.dbId) {
      try {
        await fetch(`${API_BASE_URL}/Notifications/${notif.dbId}/read`, {
          method: "PATCH",
          credentials: "include"
        });
      } catch (err) {
        console.error("Failed to mark as read:", err);
      }
    }
  };

  const markAllAsRead = async () => {
    const unread = notifications.filter(n => !n.read);
    setNotifications(prev => prev.map(n => ({ ...n, read: true })));
    for (const notif of unread) {
      if (notif.dbId) {
        try {
          await fetch(`${API_BASE_URL}/Notifications/${notif.dbId}/read`, {
            method: "PATCH",
            credentials: "include"
          });
        } catch {
          continue;
        }
      }
    }
  };

  const getIcon = (type) => {
    switch(type) {
      case "alert": return "🚨";
      case "report": return "📋";
      default: return "🔔";
    }
  };

  return (
    <div className="notification-bell">
      <button className="bell-button" onClick={() => setShowDropdown(!showDropdown)}>
        🔔 {unreadCount > 0 && <span className="notification-badge">{unreadCount}</span>}
        {isConnected && <span className="connected-dot"></span>}
      </button>

      {showDropdown && (
        <div className="notification-dropdown">
          <div className="notification-header">
            <span>Notifications</span>
            {unreadCount > 0 && (
              <button className="mark-all-read" onClick={markAllAsRead}>Mark all read</button>
            )}
          </div>
          <div className="notification-list">
            {notifications.length === 0 ? (
              <div className="no-notifications">No notifications</div>
            ) : (
              notifications.map(notif => (
                <div key={notif.id} className={`notification-item ${!notif.read ? "unread" : ""}`} onClick={() => markAsRead(notif)}>
                  <div className="notification-icon">{getIcon(notif.type)}</div>
                  <div className="notification-content">
                    <div className="notification-title">{notif.title}</div>
                    <div className="notification-message">{notif.message}</div>
                    <div className="notification-time">{notif.timestamp?.toLocaleTimeString()}</div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}