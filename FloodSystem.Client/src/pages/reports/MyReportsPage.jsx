import { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useDrainReports, useFloodReports } from "../../hooks/useReports";
import ReportCard from "../../components/reports/ReportCard";
import "./ReportPage.css";

export default function MyReportsPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("drain");
  const [statusFilter, setStatusFilter] = useState("");
  const [sortOrder, setSortOrder] = useState("newest");

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

  return (
    <div className="my-reports-page">
      <div className="my-reports-page__header">
        <div>
          <h1>📋 My Reports</h1>
          <p>All reports you have submitted</p>
        </div>
        <div className="my-reports-page__actions">
          <button className="rp-btn rp-btn--outline rp-btn--sm" onClick={() => navigate("/report/drain")}>
            + Drain Report
          </button>
          <button className="rp-btn rp-btn--flood rp-btn--sm" onClick={() => navigate("/report/flood")}>
            + Flood Report
          </button>
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