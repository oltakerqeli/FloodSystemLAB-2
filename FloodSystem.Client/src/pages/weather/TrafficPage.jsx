import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getSafeRoutes } from "../../services/weatherService";
import "./WeatherPage.css";

export default function TrafficPage() {
  const navigate = useNavigate();
  const [trafficData, setTrafficData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("all");

  useEffect(() => {
    const loadTraffic = async () => {
      setLoading(true);
      try {
        const data = await getSafeRoutes();
        setTrafficData(data);
      } catch (error) {
        console.error("Error loading traffic:", error);
      } finally {
        setLoading(false);
      }
    };
    loadTraffic();
  }, []);

  // Transformo të dhënat për listën e trafikut
  const getAllLocations = () => {
    if (!trafficData) return [];
    
    const all = [
      ...(trafficData.safeLocations || []).map(loc => ({ ...loc, status: "OPEN", statusClass: "open" })),
      ...(trafficData.cautionLocations || []).map(loc => ({ ...loc, status: "CAUTION", statusClass: "caution" })),
      ...(trafficData.blockedLocations || []).map(loc => ({ ...loc, status: "BLOCKED", statusClass: "blocked" })),
    ];
    
    if (filter === "all") return all;
    if (filter === "open") return all.filter(l => l.status === "OPEN");
    if (filter === "caution") return all.filter(l => l.status === "CAUTION");
    if (filter === "blocked") return all.filter(l => l.status === "BLOCKED");
    return all;
  };

  const locations = getAllLocations();
  const stats = {
    open: trafficData?.safeLocations?.length || 0,
    caution: trafficData?.cautionLocations?.length || 0,
    blocked: trafficData?.blockedLocations?.length || 0,
    total: (trafficData?.safeLocations?.length || 0) + 
           (trafficData?.cautionLocations?.length || 0) + 
           (trafficData?.blockedLocations?.length || 0)
  };

  if (loading) {
    return (
      <div className="wp-page">
        <div className="wp-container">
          <div className="wp-loading">Loading traffic data...</div>
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
            <div className="wp-header__icon">🚦</div>
            <div>
              <h1>Traffic Updates</h1>
              <p>Real-time road status based on flood risk assessment</p>
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
              <div className="wp-summary-pill__val">{stats.open}</div>
              <div className="wp-summary-pill__label">Open Roads</div>
            </div>
          </div>
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">⚠️</div>
            <div>
              <div className="wp-summary-pill__val">{stats.caution}</div>
              <div className="wp-summary-pill__label">Caution</div>
            </div>
          </div>
          <div className="wp-summary-pill">
            <div className="wp-summary-pill__icon">🚫</div>
            <div>
              <div className="wp-summary-pill__val">{stats.blocked}</div>
              <div className="wp-summary-pill__label">Blocked</div>
            </div>
          </div>
        </div>

        {/* Filter Buttons */}
        <div className="wp-tabs">
          <button className={`wp-tab ${filter === "all" ? "wp-tab--active" : ""}`} onClick={() => setFilter("all")}>
            All ({stats.total})
          </button>
          <button className={`wp-tab ${filter === "open" ? "wp-tab--active" : ""}`} onClick={() => setFilter("open")}>
            ✅ Open ({stats.open})
          </button>
          <button className={`wp-tab ${filter === "caution" ? "wp-tab--active" : ""}`} onClick={() => setFilter("caution")}>
            ⚠️ Caution ({stats.caution})
          </button>
          <button className={`wp-tab ${filter === "blocked" ? "wp-tab--active" : ""}`} onClick={() => setFilter("blocked")}>
            🚫 Blocked ({stats.blocked})
          </button>
        </div>

        {/* Traffic List */}
        {locations.length === 0 ? (
          <div className="wp-empty">
            <div className="wp-empty__icon">🚦</div>
            <p>No traffic data available</p>
          </div>
        ) : (
          <div className="wp-alert-list">
            {locations.map((loc) => (
              <div key={loc.id} className={`wp-alert-item wp-alert-item--${loc.statusClass}`}>
                <div className="wp-alert-icon">
                  {loc.status === "OPEN" && "✅"}
                  {loc.status === "CAUTION" && "⚠️"}
                  {loc.status === "BLOCKED" && "🚫"}
                </div>
                <div className="wp-alert-body">
                  <p className="wp-alert-body__title">{loc.name}</p>
                  <p className="wp-alert-body__msg">
                    {loc.status === "OPEN" ? "Road is open and safe for travel" : 
                     loc.status === "CAUTION" ? "Use caution due to flood risk" : 
                     "Road is blocked due to flooding"}
                  </p>
                  <div className="wp-alert-meta">
                    <span className="wp-alert-meta__zone">📍 {loc.latitude}, {loc.longitude}</span>
                  </div>
                </div>
                <div className={`wp-risk wp-risk--${loc.statusClass}`}>
                  {loc.status}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}