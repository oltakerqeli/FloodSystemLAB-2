import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  searchAlerts,
  searchLocations,
  searchWeather,
  searchTraffic,
  searchZones,
} from "../../services/searchService";
import { API_BASE_URL } from "../../utils/apiConfig";

export default function SearchPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("alerts");
  const [results, setResults] = useState([]);
  const [searched, setSearched] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [totalCount, setTotalCount] = useState(0);
  const [locations, setLocations] = useState([]);

  const [filters, setFilters] = useState({
    keyword: "",
    status: "",
    riskLevel: "",
    dateFrom: "",
    dateTo: "",
    locationId: "",
  });

  useEffect(() => {
    fetch(`${API_BASE_URL}/Locations`, { credentials: "include" })
      .then((r) => r.json())
      .then((data) => setLocations(data || []))
      .catch(() => {});
  }, []);

  const handleChange = (e) => {
    setFilters({ ...filters, [e.target.name]: e.target.value });
  };

  const handleSearch = async () => {
  setLoading(true);
  setError("");
  setResults([]);
  setSearched(false);
  setTotalCount(0);
  try {
    let response;
    if (activeTab === "alerts") {
      response = await searchAlerts({
        searchTerm: filters.keyword,
        riskLevel: filters.riskLevel || null,
        fromDate: filters.dateFrom || null,
        toDate: filters.dateTo || null,
        page: 1, pageSize: 20,
      });
    } else if (activeTab === "locations") {
      response = await searchLocations({
        searchTerm: filters.keyword,
        page: 1, pageSize: 20,
      });
    } else if (activeTab === "weather") {
      response = await searchWeather({
        locationId: filters.locationId ? parseInt(filters.locationId) : null,
        fromDate: filters.dateFrom || null,
        toDate: filters.dateTo || null,
        page: 1, pageSize: 20,
      });
    } else if (activeTab === "traffic") {
      response = await searchTraffic({
        searchTerm: filters.keyword,
        status: filters.status || null,
        fromDate: filters.dateFrom || null,
        toDate: filters.dateTo || null,
        page: 1, pageSize: 20,
      });
    } else if (activeTab === "zones") {
      response = await searchZones({
        searchTerm: filters.keyword,
        page: 1, pageSize: 20,
      });
    } else if (activeTab === "flood") {
      const data = await fetch(`${API_BASE_URL}/Reports/flood`, {
        credentials: "include",
      }).then((r) => r.json());

      const filtered = Array.isArray(data) ? data.filter((r) => {
        const matchKeyword = !filters.keyword ||
          r.description?.toLowerCase().includes(filters.keyword.toLowerCase());
        const matchStatus = !filters.status ||
          r.status?.toLowerCase() === filters.status.toLowerCase();
        return matchKeyword && matchStatus;
      }) : [];

      response = { items: filtered, totalCount: filtered.length };
    } else if (activeTab === "drain") {
      const data = await fetch(`${API_BASE_URL}/Reports/drain`, {
        credentials: "include",
      }).then((r) => r.json());

      const filtered = Array.isArray(data) ? data.filter((r) => {
        const matchKeyword = !filters.keyword ||
          r.description?.toLowerCase().includes(filters.keyword.toLowerCase());
        const matchStatus = !filters.status ||
          r.status?.toLowerCase() === filters.status.toLowerCase();
        return matchKeyword && matchStatus;
      }) : [];

      response = { items: filtered, totalCount: filtered.length };
    }

    setResults(response.items || []);
    setTotalCount(response.totalCount || 0);
    setSearched(true);
  } catch (err) {
    setError(err.message);
    setSearched(true);
  } finally {
    setLoading(false);
  }
};

  const tabs = [
    { id: "alerts", label: "🚨 Alerts" },
    { id: "locations", label: "📍 Locations" },
    { id: "weather", label: "🌤️ Weather" },
    { id: "traffic", label: "🚦 Traffic" },
    { id: "zones", label: "🗺️ Zones" },
    { id: "flood", label: "🌊 Flood" },
    { id: "drain", label: "🚧 Drain" },
  ];

  const tabColors = {
    alerts: "#ef4444",
    locations: "#3b82f6",
    weather: "#0ea5e9",
    traffic: "#f59e0b",
    zones: "#22c55e",
    flood: "#6366f1",
    drain: "#f97316",
  };

  const color = tabColors[activeTab];

  return (
    <div style={{
      minHeight: "100vh",
      background: "linear-gradient(135deg, #060d1f 0%, #0a1a3a 35%, #0d2252 65%, #060f28 100%)",
      padding: "40px 24px",
      color: "white",
    }}>
      <div style={{ maxWidth: "860px", margin: "0 auto" }}>

        {/* Header */}
        <div style={{
          display: "flex", alignItems: "center", justifyContent: "space-between",
          padding: "24px 28px", marginBottom: "28px",
          background: "rgba(255,255,255,0.05)",
          border: "1px solid rgba(255,255,255,0.08)",
          borderRadius: "20px", backdropFilter: "blur(10px)",
        }}>
          <div style={{ display: "flex", alignItems: "center", gap: "16px" }}>
            <div style={{
              width: "48px", height: "48px", borderRadius: "50%",
              background: "rgba(59,130,246,0.2)",
              border: "1px solid rgba(59,130,246,0.4)",
              display: "flex", alignItems: "center", justifyContent: "center",
              fontSize: "22px",
            }}>🔍</div>
            <div>
              <h1 style={{ fontSize: "24px", fontWeight: "800", margin: "0 0 4px", color: "white" }}>
                Advanced Search
              </h1>
              <p style={{ fontSize: "13px", color: "rgba(147,197,253,0.7)", margin: 0 }}>
                Search across all data in the system
              </p>
            </div>
          </div>

          {/* Back button - djathtas */}
          <button
            onClick={() => navigate("/dashboard")}
            style={{
              padding: "10px 20px", borderRadius: "12px",
              background: "rgba(255,255,255,0.08)",
              border: "1px solid rgba(255,255,255,0.15)",
              color: "white", fontSize: "13px", fontWeight: "600",
              cursor: "pointer", display: "flex", alignItems: "center", gap: "6px",
              whiteSpace: "nowrap",
            }}
          >
            🏠 Dashboard
          </button>
        </div>

        {/* Tabs */}
        <div style={{
          display: "flex", flexWrap: "wrap", gap: "4px", padding: "4px",
          background: "rgba(255,255,255,0.04)",
          border: "1px solid rgba(255,255,255,0.08)",
          borderRadius: "14px", marginBottom: "20px",
        }}>
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => {
                setActiveTab(tab.id);
                setResults([]);
                setSearched(false);
                setFilters({ keyword: "", status: "", riskLevel: "", dateFrom: "", dateTo: "", locationId: "" });
              }}
              style={{
                flex: "1 1 auto", padding: "10px 8px",
                fontSize: "13px", fontWeight: "600",
                color: activeTab === tab.id ? tabColors[tab.id] : "rgba(255,255,255,0.45)",
                background: activeTab === tab.id ? `${tabColors[tab.id]}18` : "transparent",
                border: activeTab === tab.id ? `1px solid ${tabColors[tab.id]}35` : "1px solid transparent",
                borderRadius: "10px", cursor: "pointer", transition: "all 0.2s",
              }}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Filters */}
        <div style={{
          padding: "20px", marginBottom: "16px",
          background: "rgba(255,255,255,0.04)",
          border: "1px solid rgba(255,255,255,0.08)",
          borderRadius: "16px",
        }}>
          <div style={{ display: "flex", flexWrap: "wrap", gap: "12px" }}>

            {activeTab !== "weather" && (
              <div style={{
                flex: "1 1 200px", height: "48px",
                display: "flex", alignItems: "center", gap: "12px",
                padding: "0 16px", borderRadius: "16px",
                background: "rgba(255,255,255,0.06)",
                border: "1px solid rgba(255,255,255,0.1)",
              }}>
                <span style={{ fontSize: "13px", opacity: 0.5 }}>🔍</span>
                <input
                  name="keyword"
                  placeholder="Search by keyword..."
                  value={filters.keyword}
                  onChange={handleChange}
                  style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "14px" }}
                />
              </div>
            )}

            {activeTab === "weather" && (
              <div style={{
                flex: "1 1 200px", height: "48px",
                display: "flex", alignItems: "center", gap: "12px",
                padding: "0 16px", borderRadius: "16px",
                background: "rgba(255,255,255,0.06)",
                border: "1px solid rgba(255,255,255,0.1)",
              }}>
                <span style={{ fontSize: "13px", opacity: 0.5 }}>📍</span>
                <select
                  name="locationId"
                  value={filters.locationId}
                  onChange={handleChange}
                  style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "14px" }}
                >
                  <option value="" style={{ background: "#0d2252" }}>Select a location...</option>
                  {locations.map((loc) => (
                    <option key={loc.id} value={loc.id} style={{ background: "#0d2252" }}>
                      {loc.name}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {activeTab === "alerts" && (
              <div style={{
                flex: "1 1 160px", height: "48px",
                display: "flex", alignItems: "center",
                padding: "0 16px", borderRadius: "16px",
                background: "rgba(255,255,255,0.06)",
                border: "1px solid rgba(255,255,255,0.1)",
              }}>
                <select name="riskLevel" value={filters.riskLevel} onChange={handleChange}
                  style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "14px" }}>
                  <option value="" style={{ background: "#0d2252" }}>All risk levels</option>
                  <option value="LOW" style={{ background: "#0d2252" }}>Low</option>
                  <option value="MEDIUM" style={{ background: "#0d2252" }}>Medium</option>
                  <option value="HIGH" style={{ background: "#0d2252" }}>High</option>
                </select>
              </div>
            )}

            {activeTab === "traffic" && (
              <div style={{
                flex: "1 1 160px", height: "48px",
                display: "flex", alignItems: "center",
                padding: "0 16px", borderRadius: "16px",
                background: "rgba(255,255,255,0.06)",
                border: "1px solid rgba(255,255,255,0.1)",
              }}>
                <select name="status" value={filters.status} onChange={handleChange}
                  style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "14px" }}>
                  <option value="" style={{ background: "#0d2252" }}>All statuses</option>
                  <option value="OPEN" style={{ background: "#0d2252" }}>OPEN</option>
                  <option value="BLOCKED" style={{ background: "#0d2252" }}>BLOCKED</option>
                </select>
              </div>
            )}

            {(activeTab === "flood" || activeTab === "drain") && (
              <div style={{
                flex: "1 1 160px", height: "48px",
                display: "flex", alignItems: "center",
                padding: "0 16px", borderRadius: "16px",
                background: "rgba(255,255,255,0.06)",
                border: "1px solid rgba(255,255,255,0.1)",
              }}>
                <select name="status" value={filters.status} onChange={handleChange}
                  style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "14px" }}>
                  <option value="" style={{ background: "#0d2252" }}>All statuses</option>
                  <option value="Pending" style={{ background: "#0d2252" }}>Pending</option>
                  <option value="In Progress" style={{ background: "#0d2252" }}>In Progress</option>
                  <option value="Resolved" style={{ background: "#0d2252" }}>Resolved</option>
                </select>
              </div>
            )}

            {activeTab !== "locations" && activeTab !== "zones" && (
              <>
                <div style={{
                  flex: "1 1 140px", height: "48px",
                  display: "flex", alignItems: "center", gap: "8px",
                  padding: "0 16px", borderRadius: "16px",
                  background: "rgba(255,255,255,0.06)",
                  border: "1px solid rgba(255,255,255,0.1)",
                }}>
                  <span style={{ fontSize: "12px", opacity: 0.5, whiteSpace: "nowrap" }}>From</span>
                  <input name="dateFrom" type="date" value={filters.dateFrom} onChange={handleChange}
                    style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "13px", colorScheme: "dark" }} />
                </div>
                <div style={{
                  flex: "1 1 140px", height: "48px",
                  display: "flex", alignItems: "center", gap: "8px",
                  padding: "0 16px", borderRadius: "16px",
                  background: "rgba(255,255,255,0.06)",
                  border: "1px solid rgba(255,255,255,0.1)",
                }}>
                  <span style={{ fontSize: "12px", opacity: 0.5, whiteSpace: "nowrap" }}>To</span>
                  <input name="dateTo" type="date" value={filters.dateTo} onChange={handleChange}
                    style={{ flex: 1, border: "none", outline: "none", background: "transparent", color: "white", fontSize: "13px", colorScheme: "dark" }} />
                </div>
              </>
            )}
          </div>

          <button
            onClick={handleSearch}
            disabled={loading}
            style={{
              marginTop: "16px", width: "100%", height: "50px",
              background: `linear-gradient(135deg, ${color}, ${color}cc)`,
              color: "white", border: "none", borderRadius: "16px",
              fontSize: "15px", fontWeight: "700", cursor: "pointer",
              boxShadow: `0 4px 20px ${color}40`,
              opacity: loading ? 0.6 : 1, transition: "all 0.2s",
            }}
          >
            {loading ? "Searching..." : "🔍 Search"}
          </button>
        </div>

        {error && (
          <div style={{
            padding: "14px 20px", borderRadius: "14px", marginBottom: "16px",
            background: "rgba(239,68,68,0.1)", border: "1px solid rgba(239,68,68,0.3)",
            color: "#fca5a5", fontSize: "14px",
          }}>
            ⚠️ {error}
          </div>
        )}

        {/* Results */}
        {results.length > 0 && (
          <div>
            <p style={{ color: "rgba(147,197,253,0.7)", fontSize: "13px", marginBottom: "16px" }}>
              Found <strong style={{ color: "white" }}>{totalCount}</strong> results
            </p>
            <div style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
              {results.map((item, index) => (
                <div key={index} style={{
                  padding: "16px 20px",
                  background: "rgba(255,255,255,0.05)",
                  border: `1px solid ${color}25`,
                  borderLeft: `4px solid ${color}`,
                  borderRadius: "16px",
                  backdropFilter: "blur(10px)",
                }}>
                  <div style={{ display: "flex", flexWrap: "wrap", gap: "8px 24px" }}>
                    {Object.entries(item).map(([key, value]) => (
                      <div key={key} style={{ fontSize: "13px" }}>
                        <span style={{ color: "rgba(147,197,253,0.6)", fontWeight: "600", textTransform: "capitalize" }}>
                          {key}:{" "}
                        </span>
                        <span style={{ color: "white" }}>
                          {typeof value === "object" ? JSON.stringify(value) : String(value)}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* No results */}
        {searched && results.length === 0 && !loading && !error && (
          <div style={{
            textAlign: "center", padding: "60px 20px",
            border: "1.5px dashed rgba(255,255,255,0.1)",
            borderRadius: "20px", background: "rgba(255,255,255,0.02)",
          }}>
            <div style={{ fontSize: "40px", opacity: 0.3, marginBottom: "12px" }}>🔍</div>
            <p style={{ color: "rgba(255,255,255,0.35)", fontSize: "14px", margin: 0 }}>
              No results found for your search criteria
            </p>
          </div>
        )}

        {/* Initial state */}
        {!searched && !loading && (
          <div style={{
            textAlign: "center", padding: "60px 20px",
            border: "1.5px dashed rgba(255,255,255,0.1)",
            borderRadius: "20px", background: "rgba(255,255,255,0.02)",
          }}>
            <div style={{ fontSize: "40px", opacity: 0.3, marginBottom: "12px" }}>🔍</div>
            <p style={{ color: "rgba(255,255,255,0.35)", fontSize: "14px", margin: 0 }}>
              Enter search criteria and click "Search"
            </p>
          </div>
        )}
      </div>
    </div>
  );
}