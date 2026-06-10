import * as signalR from "@microsoft/signalr";

class NotificationService {
  constructor() {
    this.connection = null;
    this.startPromise = null;
    this.handlers = new Map();
  }

  async connect({ userId, isAdmin } = {}) {
    if (this.startPromise) {
      return this.startPromise;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5164/notificationHub")
      .withAutomaticReconnect()
      .build();

    this.connection = connection;

    for (const [event, callbacks] of this.handlers.entries()) {
      for (const callback of callbacks) {
        connection.on(event, callback);
      }
    }

    this.startPromise = (async () => {
      await connection.start();

      if (this.connection !== connection) {
        return null;
      }

      if (connection.state !== signalR.HubConnectionState.Connected) {
        return null;
      }

      console.log("✅ SignalR connected! userId:", userId, "isAdmin:", isAdmin);
      await connection.invoke("JoinAllGroup");
      if (isAdmin) {
        await connection.invoke("JoinAdminGroup");
        console.log("Joined Admins group");
      }
      if (userId) {
        await connection.invoke("JoinUserGroup", String(userId));
        console.log("Joined user_" + userId);
      }
      return connection;
    })();

    try {
      return await this.startPromise;
    } catch (err) {
      if (this.connection === connection) {
        this.startPromise = null;
      }
      if (this.connection !== connection || err?.name === "AbortError") {
        return null;
      }
      console.error("SignalR connection error:", err);
      throw err;
    }
  }

  async disconnect() {
    const pendingStart = this.startPromise;
    this.startPromise = null;

    const connection = this.connection;
    this.connection = null;

    if (!connection) {
      return;
    }

    if (pendingStart) {
      await pendingStart.catch(() => {});
    }

    try {
      await connection.stop();
    } catch {
      return;
    }
  }

  on(event, callback) {
    if (!this.handlers.has(event)) {
      this.handlers.set(event, []);
    }
    this.handlers.get(event).push(callback);

    if (this.connection) {
      this.connection.on(event, callback);
    }
  }

  off(event, callback) {
    if (callback) {
      const callbacks = this.handlers.get(event);
      if (callbacks) {
        const index = callbacks.indexOf(callback);
        if (index !== -1) {
          callbacks.splice(index, 1);
        }
      }
      if (this.connection) {
        this.connection.off(event, callback);
      }
    } else {
      this.handlers.delete(event);
      if (this.connection) {
        this.connection.off(event);
      }
    }
  }
}

export default new NotificationService();