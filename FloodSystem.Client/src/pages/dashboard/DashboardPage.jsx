import { useEffect, useState, useCallback } from "react";
import { useNavigate, Link } from "react-router-dom";
import { MapContainer, TileLayer, Circle, CircleMarker, Popup, useMap } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { getWeatherLatestAll, getAlerts, getZones, getLocations, getSafeRoutes } from "../../services/weatherService";
import { useAuth } from "../../contexts/AuthContext";
import "./DashboardPage.css";

const RISK_COLOR = {
  low: { fill: "#22c55e", stroke: "#16a34a" },
  moderate: { fill: "#f59e0b", stroke: "#d97706" },
  medium: { fill: "#f59e0b", stroke: "#d97706" },
  high: { fill: "#f97316", stroke: "#ea580c" },
  critical: { fill: "#ef4444", stroke: "#dc2626" },
};

function riskLabel(r) {
  if (!r) return "Unknown";
  const m = { low: "🟢 Low", moderate: "🟡 Moderate", medium: "🟡 Moderate", high: "🟠 High", critical: "🔴 Critical" };
  return m[r.toLowerCase()] ?? r;
}

function riskColor(r) {
  return RISK_COLOR[r?.toLowerCase()] ?? { fill: "#94a3b8", stroke: "#64748b" };
}

function severityClass(s) {
  const m = { low: "low", moderate: "moderate", medium: "moderate", high: "high", critical: "critical" };
  return m[s?.toLowerCase()] ?? "low";
}

function FitBounds() {
  const map = useMap();
  useEffect(() => {
    map.setView([42.6629, 21.1655], 12);
  }, [map]);
  return null;
}

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [weather, setWeather] = useState([]);
  const [alerts, setAlerts] = useState([]);
  const [zones, setZones] = useState([]);
  const [locations, setLocations] = useState([]);
  const [safeRoutes, setSafeRoutes] = useState(null);
  const [loading, setLoading] = useState(true);

  const isAdmin = user?.roles?.includes("Admin");
  const isAuthority = user?.roles?.includes("Authority");
  const isAdminOrAuthority = isAdmin || isAuthority;

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const [l, z, a, w, r] = await Promise.all([
        getLocations(),
        getZones(),
        getAlerts(),
        getWeatherLatestAll(),
        getSafeRoutes().catch(() => null)
      ]);
      setLocations(l || []);
      setZones(z || []);
      setAlerts(a || []);
      setWeather(w || []);
      setSafeRoutes(r);
    } catch (error) {
      console.error("Load error:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const activeAlerts = alerts.filter((a) => a.riskLevel === "HIGH" || a.riskLevel === "MEDIUM");
const highRiskCount = alerts.filter((a) => a.riskLevel === "HIGH").length;
const mediumRiskCount = alerts.filter((a) => a.riskLevel === "MEDIUM").length;

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  const zoneMapData = zones.length > 0 ? zones.map((z, idx) => ({
    id: z.id,
    name: z.name,
    risk: z.currentRiskLevel?.toLowerCase() || "low",
    center: [42.66 + (idx * 0.01), 21.16 + (idx * 0.008)],
    radius: 1000,
    desc: z.description || z.name,
  })) : [];

  const locationCoords = locations.map((loc) => ({
    id: loc.id,
    name: loc.name,
    lat: loc.latitude,
    lng: loc.longitude,
    risk: weather.find(w => w.locationId === loc.id)?.riskLevel?.toLowerCase() || "low"
  }));

  if (loading) return <div className="db-page"><div style={{ color: "white", padding: "20px" }}>Loading...</div></div>;

  return (
    <div className="db-page">
      {/* NAVBAR */}
      <nav className="db-navbar">
        <div className="db-navbar-logo">
          <span className="db-logo-icon">🌊</span>
          <span className="db-logo-text">Flood System</span>
        </div>
        <div className="db-navbar-links">
          <Link to="/dashboard" className="db-nav-link active"><span>🏠</span> Dashboard</Link>
          <Link to="/weather" className="db-nav-link"><span>🌤️</span> Weather</Link>
          <Link to="/alerts" className="db-nav-link"><span>🚨</span> Alerts</Link>
          <Link to="/safe-routes" className="db-nav-link"><span>🛣️</span> Safe Routes</Link>
          <Link to="/my-reports" className="db-nav-link"><span>📋</span> My Reports</Link>
          <Link to="/report/flood" className="db-nav-link"><span>🌊</span> Report Flood</Link>
          <Link to="/report/drain" className="db-nav-link"><span>🚧</span> Report Drain</Link>
          {isAdminOrAuthority && (
            <Link to="/admin" className="db-nav-link"><span>⚙️</span> Authority Panel</Link>
          )}
        </div>
        <div className="db-navbar-user">
          <span className="db-user-name-nav">{user?.firstName} {user?.lastName}</span>
          <button className="db-logout-btn-nav" onClick={handleLogout}>Logout</button>
        </div>
      </nav>

      {/* MAIN CONTENT */}
      <div className="db-main">
        {/* MAP SECTION */}
        <div className="db-map-area">
          <MapContainer center={[42.6629, 21.1655]} zoom={12} className="db-map-container">
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            <FitBounds />

            {zoneMapData.map((zone) => {
              const c = riskColor(zone.risk);
              return (
                <Circle
                  key={zone.id}
                  center={zone.center}
                  radius={zone.radius}
                  pathOptions={{ color: c.stroke, fillColor: c.fill, fillOpacity: 0.15, weight: 2, dashArray: "6 4" }}
                >
                  <Popup>
                    <p className="lp-title">{zone.name}</p>
                    <p className="lp-row">📍 {zone.desc}</p>
                    <span className="lp-risk" style={{ background: c.fill + "33", color: c.fill }}>{riskLabel(zone.risk)}</span>
                  </Popup>
                </Circle>
              );
            })}

            {locationCoords.map((loc) => {
              const c = riskColor(loc.risk);
              return (
                <CircleMarker
                  key={loc.id}
                  center={[loc.lat, loc.lng]}
                  radius={10}
                  pathOptions={{ color: c.stroke, fillColor: c.fill, fillOpacity: 0.9, weight: 2 }}
                >
                  <Popup>
                    <p className="lp-title">{loc.name}</p>
                    <p className="lp-row">📍 {loc.lat}, {loc.lng}</p>
                    <span className="lp-risk" style={{ background: c.fill + "33", color: c.fill }}>{riskLabel(loc.risk)}</span>
                  </Popup>
                </CircleMarker>
              );
            })}
          </MapContainer>

          <div className="db-map-overlay">
            <div className="db-map-title">🗺️ Flood Risk Map — Kosovo</div>
            <div className="db-legend">
              <div className="db-legend__title">Risk Level</div>
              {[
                { color: "#22c55e", label: "Low Risk" },
                { color: "#f59e0b", label: "Moderate" },
                { color: "#f97316", label: "High Risk" },
                { color: "#ef4444", label: "Critical" },
              ].map((l) => (
                <div key={l.label} className="db-legend__item">
                  <div className="db-legend__dot" style={{ background: l.color }} />
                  {l.label}
                </div>
              ))}
            </div>
          </div>
        </div>

 {/* SIDEBAR */}
<aside className="db-sidebar">
  {/* User Info */}
  <div className="db-user-card">
    <div className="db-user-avatar">👤</div>
    <div className="db-user-info">
      <p className="db-user-name">{user?.firstName} {user?.lastName}</p>
      <p className="db-user-email">{user?.email}</p>
      <p className="db-user-role">{user?.roles?.join(", ")}</p>
    </div>
  </div>

  {/* EMERGENCY GUIDE */}
  <div className="db-emergency-guide">
    <p className="db-section-title">🆘 EMERGENCY GUIDE</p>
    <div className="db-guide-content">
      <div className="db-guide-item">
        <span className="db-guide-icon">🌊</span>
        <div className="db-guide-text">
          <strong>Flood:</strong> Move to higher ground immediately. Avoid walking or driving through flood water.
        </div>
      </div>
      <div className="db-guide-item">
        <span className="db-guide-icon">🚧</span>
        <div className="db-guide-text">
          <strong>Drain Blockage:</strong> Report to municipality. Avoid area until cleared.
        </div>
      </div>
      <div className="db-guide-item">
        <span className="db-guide-icon">⚠️</span>
        <div className="db-guide-text">
          <strong>High Risk Zone:</strong> Follow evacuation orders. Keep emergency kit ready.
        </div>
      </div>
    </div>
    <div className="db-emergency-calls">
      <p className="db-call-title">📞 EMERGENCY CONTACTS</p>
      <div className="db-call-item">
        <span className="db-call-number">112</span>
        <span className="db-call-label">General Emergency</span>
      </div>
      <div className="db-call-item">
        <span className="db-call-number">0800 11 222</span>
        <span className="db-call-label">Municipality Helpdesk</span>
      </div>
      <div className="db-call-item">
        <span className="db-call-number">+383 38 123 456</span>
        <span className="db-call-label">Flood Response Unit</span>
      </div>
    </div>
  </div>

  {/* OVERVIEW STATS */}
  <div>
    <p className="db-section-title">📊 OVERVIEW</p>
    <div className="db-stat-grid">
      <div className="db-stat-card">
        <div className="db-stat-card__icon">📍</div>
        <div className="db-stat-card__val">{locations.length}</div>
        <div className="db-stat-card__label">Locations</div>
      </div>
      <div className="db-stat-card">
        <div className="db-stat-card__icon">🚨</div>
        <div className="db-stat-card__val" style={{ color: activeAlerts.length ? "#fca5a5" : "white" }}>
          {activeAlerts.length}
        </div>
        <div className="db-stat-card__label">Active Alerts</div>
      </div>
      <div className="db-stat-card">
        <div className="db-stat-card__icon">🔴</div>
        <div className="db-stat-card__val">{highRiskCount}</div>
        <div className="db-stat-card__label">High Risk</div>
      </div>
      <div className="db-stat-card">
        <div className="db-stat-card__icon">🟡</div>
        <div className="db-stat-card__val">{mediumRiskCount}</div>
        <div className="db-stat-card__label">Medium Risk</div>
      </div>
    </div>
  </div>

  {/* RISK ZONES */}
  <div>
    <p className="db-section-title">🗺️ RISK ZONES</p>
    <div className="db-zone-list">
      {zones.map((z) => {
        const c = riskColor(z.currentRiskLevel?.toLowerCase() || "low");
        return (
          <div key={z.id} className="db-zone-item">
            <div className="db-zone-dot" style={{ background: c.fill }} />
            <span className="db-zone-name">{z.name}</span>
            <span className="db-zone-badge" style={{ background: c.fill + "25", color: c.fill }}>
              {riskLabel(z.currentRiskLevel)}
            </span>
          </div>
        );
      })}
    </div>
  </div>

  {/* ACTIVE ALERTS */}
  {activeAlerts.length > 0 && (
    <div>
      <p className="db-section-title">🚨 ACTIVE ALERTS ({activeAlerts.length})</p>
      <div className="db-alert-list">
        {activeAlerts.slice(0, 5).map((a) => (
          <div key={a.id} className={`db-alert-item db-alert-item--${severityClass(a.riskLevel)}`}>
            <div className="db-alert-content">
              <p className="db-alert-title">{a.type}</p>
              <p className="db-alert-msg">{a.message?.substring(0, 50)}...</p>
              <p className="db-alert-location">📍 {a.locationName}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  )}

  {/* SAFE ROUTES SUMMARY */}
  {safeRoutes && (
    <div>
      <p className="db-section-title">🛣️ SAFE ROUTES SUMMARY</p>
      <div className="db-stat-grid">
        <div className="db-stat-card">
          <div className="db-stat-card__icon">✅</div>
          <div className="db-stat-card__val">{safeRoutes.safeLocations?.length || 0}</div>
          <div className="db-stat-card__label">Safe</div>
        </div>
        <div className="db-stat-card">
          <div className="db-stat-card__icon">⚠️</div>
          <div className="db-stat-card__val">{safeRoutes.cautionLocations?.length || 0}</div>
          <div className="db-stat-card__label">Caution</div>
        </div>
        <div className="db-stat-card">
          <div className="db-stat-card__icon">🚫</div>
          <div className="db-stat-card__val">{safeRoutes.blockedLocations?.length || 0}</div>
          <div className="db-stat-card__label">Blocked</div>
        </div>
      </div>
    </div>
  )}
</aside>
      </div>
    </div>
  );
}