import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getLocations, createLocation, updateLocation, deleteLocation, getZones, createZone, updateZone, deleteZone } from "../../services/weatherService";
import { useAuth } from "../../contexts/AuthContext";
import { API_BASE_URL } from "../../utils/apiConfig";
import "./AdminPanelPage.css";

export default function AdminPanelPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [locations, setLocations] = useState([]);
  const [zones, setZones] = useState([]);
  const [activeTab, setActiveTab] = useState("locations");
  const [loading, setLoading] = useState(true);
  const [editingLocation, setEditingLocation] = useState(null);
  const [editingZone, setEditingZone] = useState(null);
  const [allFloodReports, setAllFloodReports] = useState([]);
  const [allDrainReports, setAllDrainReports] = useState([]);
  const [reportsTab, setReportsTab] = useState("drain");
  const [form, setForm] = useState({ name: "", description: "", latitude: "", longitude: "" });
  const [zoneForm, setZoneForm] = useState({ name: "", description: "", criticalRainfallThreshold: 10 });

  const isAdmin = user?.roles?.includes("Admin");
  const isAuthority = user?.roles?.includes("Authority");
  const canEdit = isAdmin || isAuthority;
  const canDelete = isAdmin || isAuthority;

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const [l, z] = await Promise.all([getLocations(), getZones()]);
      setLocations(Array.isArray(l) ? l : []);
      setZones(Array.isArray(z) ? z : []);

      const floodRes = await fetch(`${API_BASE_URL}/Reports/flood/all`, { credentials: "include" });
      const drainRes = await fetch(`${API_BASE_URL}/Reports/drain/all`, { credentials: "include" });
      if (floodRes.ok) setAllFloodReports(await floodRes.json());
      if (drainRes.ok) setAllDrainReports(await drainRes.json());
    } catch (error) {
      console.error("Failed to load data:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleStatusChange = async (id, statusId, type) => {
    try {
      const response = await fetch(
        `${API_BASE_URL}/Reports/${id}/status?statusId=${statusId}&type=${type}`,
        { method: "PATCH", credentials: "include" }
      );
      if (!response.ok) throw new Error("Failed");
      await loadData();
    } catch (err) {
      alert("Failed to update status");
    }
  };

  const handleCreateLocation = async (e) => {
    e.preventDefault();
    if (!canEdit) return alert("You don't have permission");
    try {
      await createLocation({
        name: form.name,
        description: form.description,
        latitude: parseFloat(form.latitude),
        longitude: parseFloat(form.longitude),
      });
      setForm({ name: "", description: "", latitude: "", longitude: "" });
      await loadData();
      alert("Location created");
    } catch (error) {
      alert("Failed to create location");
    }
  };

  const handleUpdateLocation = async (e) => {
    e.preventDefault();
    if (!canEdit) return alert("You don't have permission");
    try {
      await updateLocation(editingLocation.id, {
        name: form.name,
        description: form.description,
        latitude: parseFloat(form.latitude),
        longitude: parseFloat(form.longitude),
      });
      setEditingLocation(null);
      setForm({ name: "", description: "", latitude: "", longitude: "" });
      await loadData();
      alert("Location updated");
    } catch (error) {
      alert("Failed to update location");
    }
  };

  const handleDeleteLocation = async (id) => {
    if (!canDelete) return alert("You don't have permission");
    if (window.confirm("Are you sure you want to permanently delete this location?")) {
      try {
        await deleteLocation(id);
        await loadData();
        alert("Location permanently deleted");
      } catch (error) {
        alert("Failed to delete: " + error.message);
      }
    }
  };

  const startEditLocation = (loc) => {
    setEditingLocation(loc);
    setForm({
      name: loc.name,
      description: loc.description || "",
      latitude: loc.latitude,
      longitude: loc.longitude,
    });
  };

  const cancelEditLocation = () => {
    setEditingLocation(null);
    setForm({ name: "", description: "", latitude: "", longitude: "" });
  };

  const handleCreateZone = async (e) => {
    e.preventDefault();
    if (!canEdit) return alert("You don't have permission");
    try {
      await createZone({
        name: zoneForm.name,
        description: zoneForm.description,
        criticalRainfallThreshold: parseFloat(zoneForm.criticalRainfallThreshold),
      });
      setZoneForm({ name: "", description: "", criticalRainfallThreshold: 10 });
      await loadData();
      alert("Zone created");
    } catch (error) {
      alert("Failed to create zone");
    }
  };

  const handleUpdateZone = async (e) => {
    e.preventDefault();
    if (!canEdit) return alert("You don't have permission");
    try {
      await updateZone(editingZone.id, {
        name: zoneForm.name,
        description: zoneForm.description,
        criticalRainfallThreshold: parseFloat(zoneForm.criticalRainfallThreshold),
      });
      setEditingZone(null);
      setZoneForm({ name: "", description: "", criticalRainfallThreshold: 10 });
      await loadData();
      alert("Zone updated");
    } catch (error) {
      alert("Failed to update zone");
    }
  };

  const handleDeleteZone = async (id) => {
    if (!canDelete) return alert("Only Admin can delete");
    if (window.confirm("Are you sure?")) {
      await deleteZone(id);
      await loadData();
      alert("Zone deleted");
    }
  };

  const startEditZone = (zone) => {
    setEditingZone(zone);
    setZoneForm({
      name: zone.name,
      description: zone.description || "",
      criticalRainfallThreshold: zone.criticalRainfallThreshold,
    });
  };

  const cancelEditZone = () => {
    setEditingZone(null);
    setZoneForm({ name: "", description: "", criticalRainfallThreshold: 10 });
  };

  return (
    <div className="admin-page">
      <div className="admin-container">
        <div className="admin-header">
          <h1>Admin Panel</h1>
          <button className="admin-back-btn" onClick={() => navigate("/dashboard")}>← Back</button>
        </div>

        <div className="admin-tabs">
          <button className={`admin-tab ${activeTab === "locations" ? "active" : ""}`} onClick={() => setActiveTab("locations")}>
            📍 Locations ({locations.length})
          </button>
          <button className={`admin-tab ${activeTab === "zones" ? "active" : ""}`} onClick={() => setActiveTab("zones")}>
            🗺️ Zones ({zones.length})
          </button>
          <button className={`admin-tab ${activeTab === "reports" ? "active" : ""}`} onClick={() => setActiveTab("reports")}>
            📋 Reports ({allFloodReports.length + allDrainReports.length})
          </button>
          <button className={`admin-tab ${activeTab === "users" ? "active" : ""}`} onClick={() => navigate("/admin/users")}>
            👥 Manage Users
          </button>
        </div>

        {activeTab === "locations" && (
          <div className="admin-section">
            {canEdit && (
              <form className="admin-form" onSubmit={editingLocation ? handleUpdateLocation : handleCreateLocation}>
                <h3>{editingLocation ? "Edit Location" : "Add New Location"}</h3>
                <input type="text" placeholder="Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
                <input type="text" placeholder="Description" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
                <input type="number" step="0.0001" placeholder="Latitude" value={form.latitude} onChange={(e) => setForm({ ...form, latitude: e.target.value })} required />
                <input type="number" step="0.0001" placeholder="Longitude" value={form.longitude} onChange={(e) => setForm({ ...form, longitude: e.target.value })} required />
                <div style={{ display: "flex", gap: "10px" }}>
                  <button type="submit">{editingLocation ? "Update" : "Create"}</button>
                  {editingLocation && <button type="button" onClick={cancelEditLocation}>Cancel</button>}
                </div>
              </form>
            )}
            <div className="admin-list">
              <h3>Existing Locations</h3>
              {loading ? <div>Loading...</div> : locations.map(loc => (
                <div key={loc.id} className="admin-item">
                  <div>
                    <strong>{loc.name}</strong>
                    <span className="admin-coords">{loc.latitude}, {loc.longitude}</span>
                    {loc.description && <span className="admin-desc">{loc.description}</span>}
                  </div>
                  <div style={{ display: "flex", gap: "8px" }}>
                    {canEdit && <button className="admin-edit" onClick={() => startEditLocation(loc)}>Edit</button>}
                    {canDelete && <button className="admin-delete" onClick={() => handleDeleteLocation(loc.id)}>Delete</button>}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {activeTab === "zones" && (
          <div className="admin-section">
            {canEdit && (
              <form className="admin-form" onSubmit={editingZone ? handleUpdateZone : handleCreateZone}>
                <h3>{editingZone ? "Edit Zone" : "Add New Zone"}</h3>
                <input type="text" placeholder="Name" value={zoneForm.name} onChange={(e) => setZoneForm({ ...zoneForm, name: e.target.value })} required />
                <input type="text" placeholder="Description" value={zoneForm.description} onChange={(e) => setZoneForm({ ...zoneForm, description: e.target.value })} />
                <input type="number" step="0.1" placeholder="Threshold (mm)" value={zoneForm.criticalRainfallThreshold} onChange={(e) => setZoneForm({ ...zoneForm, criticalRainfallThreshold: e.target.value })} required />
                <div style={{ display: "flex", gap: "10px" }}>
                  <button type="submit">{editingZone ? "Update" : "Create"}</button>
                  {editingZone && <button type="button" onClick={cancelEditZone}>Cancel</button>}
                </div>
              </form>
            )}
            <div className="admin-list">
              <h3>Existing Zones</h3>
              {loading ? <div>Loading...</div> : zones.map(zone => (
                <div key={zone.id} className="admin-item">
                  <div>
                    <strong>{zone.name}</strong>
                    <span className="admin-desc">{zone.description}</span>
                    <span className="admin-threshold">Threshold: {zone.criticalRainfallThreshold} mm</span>
                  </div>
                  <div style={{ display: "flex", gap: "8px" }}>
                    {canEdit && <button className="admin-edit" onClick={() => startEditZone(zone)}>Edit</button>}
                    {canDelete && <button className="admin-delete" onClick={() => handleDeleteZone(zone.id)}>Delete</button>}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {activeTab === "reports" && (
          <div className="admin-section">
            <div className="admin-tabs" style={{ marginBottom: "16px" }}>
              <button className={`admin-tab ${reportsTab === "drain" ? "active" : ""}`} onClick={() => setReportsTab("drain")}>
                🚧 Drain Reports ({allDrainReports.length})
              </button>
              <button className={`admin-tab ${reportsTab === "flood" ? "active" : ""}`} onClick={() => setReportsTab("flood")}>
                🌊 Flood Reports ({allFloodReports.length})
              </button>
            </div>
            <div className="admin-list">
              {(reportsTab === "drain" ? allDrainReports : allFloodReports).map(report => (
                <div key={report.id} className="admin-item">
                  <div>
                    <strong>{report.description}</strong>
                    <span className="admin-desc">📍 {report.street}, {report.district}</span>
                    <span className="admin-desc">⚠️ Severity: {report.severity}</span>
                  </div>
                  <div style={{ display: "flex", gap: "8px", alignItems: "center" }}>
                    <span style={{
                      fontSize: "11px", fontWeight: "700", padding: "4px 10px",
                      borderRadius: "20px", background: "rgba(255,255,255,0.1)",
                      color: "white"
                    }}>
                      {report.status}
                    </span>
                    <select
                      value=""
                      onChange={(e) => handleStatusChange(report.id, e.target.value, reportsTab === "drain" ? "Drain" : "Flood")}
                      style={{
                        padding: "4px 8px", borderRadius: "8px",
                        background: "rgba(255,255,255,0.1)",
                        border: "1px solid rgba(255,255,255,0.2)",
                        color: "white", fontSize: "12px"
                      }}
                    >
                      <option value="">Change status</option>
                      <option value="1">Pending</option>
                      <option value="2">Reviewed</option>
                      <option value="3">Resolved</option>
                    </select>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}