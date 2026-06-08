import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getSafeRoutes } from "../../services/weatherService";
import "./WeatherPage.css";

export default function SafeRoutesPage() {
  const navigate = useNavigate();
  const [routes, setRoutes] = useState(null);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("safe");

  useEffect(() => {
    const loadRoutes = async () => {
      setLoading(true);
      try {
        const data = await getSafeRoutes();
        setRoutes(data);
      } catch (error) {
        console.error("Error loading safe routes:", error);
      } finally {
        setLoading(false);
      }
    };
    loadRoutes();
  }, []);

  const getFilteredRoutes = () => {
    if (!routes) return [];
    
    if (filter === "safe") return routes.safeLocations || [];
    if (filter === "caution") return routes.cautionLocations || [];
    if (filter === "blocked") return routes.blockedLocations || [];
    return [];
  };

  const filteredRoutes = getFilteredRoutes();
  const stats = {
    safe: routes?.safeLocations?.length || 0,
    caution: routes?.cautionLocations?.length || 0,
    blocked: routes?.blockedLocations?.length || 0,
  };

  if (loading) {
    return (
      <div className="wp-page">
        <div className="wp-container">
          <div className="wp-loading">Loading safe routes...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="wp-page">
      <div className="wp-container">
        {/* Header */}
        <div className="wp-header">
          <div className="wp-header__left">
            <div className="wp-header__icon">🛣️</div>
            <div>
              <h1>Safe Routes</h1>
              <p>Recommended routes based on current flood risk</p>
            </div>
          </div>
          <button className="wp-btn wp-btn--outline" onClick={() => navigate("/dashboard")}>
            ← Back to Dashboard
          </button>
        </div>

        {/* Stats Summary */}
        <div className="wp-summary">
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">✅</div>
            <div>
              <div className="wp-summary-pill__val">{stats.safe}</div>
              <div className="wp-summary-pill__label">Safe Routes</div>
            </div>
          </div>
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">⚠️</div>
            <div>
              <div className="wp-summary-pill__val">{stats.caution}</div>
              <div className="wp-summary-pill__label">Use Caution</div>
            </div>
          </div>
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">🚫</div>
            <div>
              <div className="wp-summary-pill__val">{stats.blocked}</div>
              <div className="wp-summary-pill__label">Avoid</div>
            </div>
          </div>
        </div>

        {/* Filter Tabs */}
        <div className="wp-tabs">
          <button className={`wp-tab ${filter === "safe" ? "wp-tab--active" : ""}`} onClick={() => setFilter("safe")}>
            ✅ Safe ({stats.safe})
          </button>
          <button className={`wp-tab ${filter === "caution" ? "wp-tab--active" : ""}`} onClick={() => setFilter("caution")}>
            ⚠️ Caution ({stats.caution})
          </button>
          <button className={`wp-tab ${filter === "blocked" ? "wp-tab--active" : ""}`} onClick={() => setFilter("blocked")}>
            🚫 Blocked ({stats.blocked})
          </button>
        </div>

        {/* Routes List */}
        {filteredRoutes.length === 0 ? (
          <div className="wp-empty">
            <div className="wp-empty__icon">
              {filter === "safe" && "✅"}
              {filter === "caution" && "⚠️"}
              {filter === "blocked" && "🚫"}
            </div>
            <p>No {filter} routes found</p>
          </div>
        ) : (
          <div className="wp-alert-list">
            {filteredRoutes.map((loc) => (
              <div key={loc.id} className={`wp-alert-item wp-alert-item--${filter === "safe" ? "low" : filter === "caution" ? "moderate" : "high"}`}>
                <div className="wp-alert-icon">
                  {filter === "safe" && "✅"}
                  {filter === "caution" && "⚠️"}
                  {filter === "blocked" && "🚫"}
                </div>
                <div className="wp-alert-body">
                  <p className="wp-alert-body__title">{loc.name}</p>
                  <p className="wp-alert-body__msg">
                    {loc.description || (
                      filter === "safe" ? "Road is safe for travel" : 
                      filter === "caution" ? "Use caution due to flood risk" : 
                      "Road is blocked - find alternative route"
                    )}
                  </p>
                  <div className="wp-alert-meta">
                    <span className="wp-alert-meta__zone">📍 {loc.latitude}, {loc.longitude}</span>
                  </div>
                </div>
                <div className={`wp-risk wp-risk--${filter === "safe" ? "low" : filter === "caution" ? "moderate" : "high"}`}>
                  {filter === "safe" ? "SAFE" : filter === "caution" ? "CAUTION" : "BLOCKED"}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}