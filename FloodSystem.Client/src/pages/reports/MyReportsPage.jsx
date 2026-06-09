import { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useDrainReports, useFloodReports } from "../../hooks/useReports";
import ReportCard from "../../components/reports/ReportCard";
import { API_BASE_URL } from "../../utils/apiConfig";
import "./ReportPage.css";

export default function MyReportsPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("drain");
  const [statusFilter, setStatusFilter] = useState("");
  const [sortOrder, setSortOrder] = useState("newest");
  const [exporting, setExporting] = useState(false);
  const [exportType, setExportType] = useState("all");

  const { reports: drainReports, loading: loadingDrain } = useDrainReports();
  const { reports: floodReports, loading: loadingFlood } = useFloodReports();

  const allReports = activeTab === "drain" ? drainReports : floodReports;
  const loading = activeTab === "drain" ? loadingDrain : loadingFlood;

  const filteredReports = useMemo(() => {
    let result = [...allReports];
    if (statusFilter) {
      result = result.filter(r => r.status?.toLowerCase() === statusFilter.toLowerCase());
    }
    result.sort((a, b) => {
      const dateA = new Date(a.createdAt);
      const dateB = new Date(b.createdAt);
      return sortOrder === "newest" ? dateB - dateA : dateA - dateB;
    });
    return result;
  }, [allReports, statusFilter, sortOrder]);

  const handleExport = async (format) => {
    try {
      setExporting(true);
      const response = await fetch(`${API_BASE_URL}/Dashboard/export`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ type: exportType, format }),
      });
      if (!response.ok) throw new Error("Export failed");
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `${exportType}_export.${format === "excel" ? "xlsx" : format}`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      alert("Export failed. Please try again.");
    } finally {
      setExporting(false);
    }
  };

  const handleImport = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      const formData = new FormData();
      formData.append("file", file);
      formData.append("type", exportType);
      const response = await fetch(`${API_BASE_URL}/Dashboard/import`, {
        method: "POST",
        credentials: "include",
        body: formData,
      });
      if (!response.ok) throw new Error("Import failed");
      alert("Import successful!");
      window.location.reload();
    } catch (err) {
      alert("Import failed. Please try again.");
    }
  };

  return (
    <div className="my-reports-page">
      <div className="my-reports-page__header">
        <div>
          
          <h1>📋 My Reports</h1>
          <p>All reports you have submitted</p>
        </div>
        <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => navigate("/dashboard")}>
  ← Dashboard
</button>
        <div className="my-reports-page__actions">
          <div style={{ display: "flex", gap: "8px", flexWrap: "wrap" }}>
            <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => navigate("/report/drain")}>
              + Drain Report
            </button>
            <button className="rp-btn rp-btn--flood rp-btn--sm" onClick={() => navigate("/report/flood")}>
              + Flood Report
            </button>
            <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => navigate("/dynamic-report")}>
              📊 Dynamic Report
            </button>
          </div>
          <div style={{ display: "flex", gap: "8px", flexWrap: "wrap", alignItems: "center" }}>
            <select
              className="export-select"
              value={exportType}
              onChange={(e) => setExportType(e.target.value)}
              style={{
                height: "34px", padding: "0 10px", borderRadius: "10px",
                background: "#0d2252", border: "1px solid rgba(255,255,255,0.15)",
                color: "white", fontSize: "13px", outline: "none", cursor: "pointer"
              }}
            >
              <option value="all">All Data</option>
              <option value="flood">Flood Reports</option>
              <option value="drain">Drain Reports</option>
              <option value="alerts">Alerts</option>
              <option value="locations">Locations</option>
            </select>
            <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => handleExport("csv")} disabled={exporting}>
              📥 CSV
            </button>
            <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => handleExport("excel")} disabled={exporting}>
              📊 Excel
            </button>
            <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => handleExport("json")} disabled={exporting}>
              📋 JSON
            </button>
            <label
              style={{
                height: "34px", padding: "0 14px", borderRadius: "10px",
                background: "rgba(255,255,255,0.08)", border: "1px solid rgba(255,255,255,0.15)",
                color: "white", fontSize: "13px", cursor: "pointer",
                display: "inline-flex", alignItems: "center", gap: "6px"
              }}
            >
              📤 Import
              <input type="file" accept=".csv,.xlsx,.json" style={{ display: "none" }} onChange={handleImport} />
            </label>
          </div>
        </div>
      </div>
      

      <div className="report-tabs">
        <button className={`report-tab ${activeTab === "drain" ? "report-tab--active" : ""}`} onClick={() => setActiveTab("drain")}>
          🚧 Drain Reports
          {drainReports.length > 0 && <span className="report-tab__count">{drainReports.length}</span>}
        </button>
        <button className={`report-tab ${activeTab === "flood" ? "report-tab--active report-tab--flood" : ""}`} onClick={() => setActiveTab("flood")}>
          🌊 Flood Reports
          {floodReports.length > 0 && <span className="report-tab__count">{floodReports.length}</span>}
        </button>
      </div>

      <div className="report-filter">
        <label>Status:</label>
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All</option>
          <option value="pending">Pending</option>
          <option value="reviewed">Reviewed</option>
          <option value="resolved">Resolved</option>
        </select>
        <label style={{ marginLeft: "12px" }}>Sort:</label>
        <select value={sortOrder} onChange={(e) => setSortOrder(e.target.value)}>
          <option value="newest">Newest first</option>
          <option value="oldest">Oldest first</option>
        </select>
      </div>
      

      <div className="report-list">
        {loading ? (
          <div className="report-list__loading">
            {[1, 2, 3].map((i) => <div key={i} className="report-skeleton" />)}
          </div>
        ) : filteredReports.length === 0 ? (
          <div className="report-list__empty">
            <div className="report-list__empty-icon">{activeTab === "drain" ? "🚧" : "🌊"}</div>
            <p>
              {statusFilter
                ? `No ${statusFilter} reports found`
                : `You have no ${activeTab === "drain" ? "drain" : "flood"} reports yet`}
            </p>
            <button className="rp-btn rp-btn--primary rp-btn--sm" onClick={() => navigate(activeTab === "drain" ? "/report/drain" : "/report/flood")}>
              Submit report
            </button>
          </div>
        ) : (
          filteredReports.map((report) => (
            <ReportCard key={report.id} report={report} type={activeTab} />
          ))
        )}
      </div>
    </div>
  );
}