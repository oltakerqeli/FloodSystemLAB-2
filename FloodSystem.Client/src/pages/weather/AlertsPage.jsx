import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getAlerts, getActiveAlerts } from "../../services/weatherService";
import "./WeatherPage.css";

function severityClass(s) {
  if (!s) return "low";
  const l = s.toLowerCase();
  if (l === "high") return "high";
  if (l === "medium") return "moderate";
  return "low";
}

function severityIcon(s) {
  if (!s) return "🟢";
  const l = s.toLowerCase();
  if (l === "high") return "🔴";
  if (l === "medium") return "🟠";
  return "🟢";
}

function fmtTime(ts) {
  if (!ts) return "—";
  return new Date(ts).toLocaleString("sq-AL");
}

export default function AlertsPage() {
  const navigate = useNavigate();
  const [alerts, setAlerts] = useState([]);
  const [filter, setFilter] = useState("all");
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const data = filter === "active" ? await getActiveAlerts() : await getAlerts();
      setAlerts(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error("Failed to load alerts:", error);
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const activeCount = alerts.filter(a => a.riskLevel?.toLowerCase() === "high").length;

  return (
    <div className="wp-page">
      <div className="wp-container">
        <div className="wp-header">
          <div className="wp-header__left">
            <div className="wp-header__icon">🚨</div>
            <div>
              <h1>Alerts</h1>
              <p>Active flood and weather alerts</p>
            </div>
          </div>
          <div className="wp-header__actions">
            <button className="wp-btn wp-btn--outline" onClick={() => navigate("/dashboard")}>
              ← Back to Dashboard
            </button>
          </div>
        </div>

        {/* Stats */}
        <div className="wp-summary">
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">🚨</div>
            <div>
              <div className="wp-summary-pill__val" style={{ color: activeCount ? "#fca5a5" : "white" }}>
                {activeCount}
              </div>
              <div className="wp-summary-pill__label">Active High Risk</div>
            </div>
          </div>
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">📋</div>
            <div>
              <div className="wp-summary-pill__val">{alerts.length}</div>
              <div className="wp-summary-pill__label">Total Alerts</div>
            </div>
          </div>
        </div>

        {/* Filter Tabs */}
        <div className="wp-tabs">
          <button
            className={`wp-tab ${filter === "all" ? "wp-tab--active" : ""}`}
            onClick={() => setFilter("all")}
          >
            All Alerts
          </button>
          <button
            className={`wp-tab wp-tab--alert ${filter === "active" ? "wp-tab--active" : ""}`}
            onClick={() => setFilter("active")}
          >
            Active (High Risk)
          </button>
        </div>

        {/* Alerts List */}
        {loading ? (
          <div className="wp-skeleton-list">
            {[1, 2, 3].map(i => <div key={i} className="wp-skeleton" />)}
          </div>
        ) : alerts.length === 0 ? (
          <div className="wp-empty">
            <div className="wp-empty__icon">✅</div>
            <p>No alerts found</p>
          </div>
        ) : (
          <div className="wp-alert-list">
            {alerts.map((a) => (
              <div
                key={a.id}
                className={`wp-alert-item wp-alert-item--${severityClass(a.riskLevel)}`}
              >
                <div className="wp-alert-icon">
                  {severityIcon(a.riskLevel)}
                </div>
                <div className="wp-alert-body">
                  <p className="wp-alert-body__title">{a.type}</p>
                  <p className="wp-alert-body__msg">{a.message}</p>
                  <div className="wp-alert-meta">
                    <span className="wp-alert-meta__zone">📍 {a.locationName}</span>
                    <span className="wp-alert-meta__time">{fmtTime(a.createdAt)}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}