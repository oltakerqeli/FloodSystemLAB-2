import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { API_BASE_URL } from "../../utils/apiConfig";
import { FaCalendarAlt, FaFilter, FaFileExcel, FaFileCsv, FaFileCode } from "react-icons/fa";
import "./ReportPage.css";

export default function DynamicReportPage() {
  const navigate = useNavigate();
  const [generating, setGenerating] = useState(false);
  const [form, setForm] = useState({
    reportType: "all",
    dateFrom: "",
    dateTo: "",
    severity: "",
    status: "",
    format: "excel",
  });

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleGenerate = async () => {
    try {
      setGenerating(true);
      const body = {
        reportType: form.reportType,
        dateFrom: form.dateFrom ? new Date(form.dateFrom).toISOString() : null,
        dateTo: form.dateTo ? new Date(form.dateTo).toISOString() : null,
        severity: form.severity || null,
        status: form.status || null,
        format: form.format,
      };

      const response = await fetch(`${API_BASE_URL}/Dashboard/dynamic-report`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(body),
      });

      if (!response.ok) throw new Error("Failed to generate report");

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `dynamic_report.${form.format === "excel" ? "xlsx" : form.format}`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      alert("Failed to generate report. Please try again.");
    } finally {
      setGenerating(false);
    }
  };

  return (
    <div className="rp-page">
      <div className="rp-card">
        <div className="rp-header">
          <div className="rp-bubble rp-bubble--flood">📊</div>
          <div>
            <h1 className="rp-title">Dynamic Report</h1>
            <p className="rp-subtitle">Generate a custom report based on your filters</p>
          </div>
        </div>

        <div className="rp-divider" />

        <div className="rp-form">

          <div className="rp-field-group">
            <label className="rp-label">Report Type</label>
            <div className="rp-field">
              <div className="rp-field-icon"><FaFilter /></div>
              <select name="reportType" value={form.reportType} onChange={handleChange}>
                <option value="all">All Reports</option>
                <option value="flood">Flood Reports Only</option>
                <option value="drain">Drain Reports Only</option>
              </select>
            </div>
          </div>

          <div className="rp-row">
            <div className="rp-field-group">
              <label className="rp-label">Date From</label>
              <div className="rp-field">
                <div className="rp-field-icon"><FaCalendarAlt /></div>
                <input type="date" name="dateFrom" value={form.dateFrom} onChange={handleChange} />
              </div>
            </div>

            <div className="rp-field-group">
              <label className="rp-label">Date To</label>
              <div className="rp-field">
                <div className="rp-field-icon"><FaCalendarAlt /></div>
                <input type="date" name="dateTo" value={form.dateTo} onChange={handleChange} />
              </div>
            </div>
          </div>

          <div className="rp-row">
            <div className="rp-field-group">
              <label className="rp-label">Severity</label>
              <div className="rp-field">
                <div className="rp-field-icon">⚠️</div>
                <select name="severity" value={form.severity} onChange={handleChange}>
                  <option value="">All</option>
                  <option value="low">🟢 Low</option>
                  <option value="medium">🟡 Medium</option>
                  <option value="high">🟠 High</option>
                  <option value="critical">🔴 Critical</option>
                </select>
              </div>
            </div>

            <div className="rp-field-group">
              <label className="rp-label">Status</label>
              <div className="rp-field">
                <div className="rp-field-icon">📌</div>
                <select name="status" value={form.status} onChange={handleChange}>
                  <option value="">All</option>
                  <option value="pending">Pending</option>
                  <option value="reviewed">Reviewed</option>
                  <option value="resolved">Resolved</option>
                </select>
              </div>
            </div>
          </div>

          <div className="rp-field-group">
            <label className="rp-label">Export Format</label>
            <div style={{ display: "flex", gap: "10px" }}>
              {[
                { value: "excel", label: "Excel", icon: <FaFileExcel /> },
                { value: "csv", label: "CSV", icon: <FaFileCsv /> },
                { value: "json", label: "JSON", icon: <FaFileCode /> },
              ].map((f) => (
                <button
                  key={f.value}
                  type="button"
                  onClick={() => setForm((prev) => ({ ...prev, format: f.value }))}
                  className={`rp-btn rp-btn--sm ${form.format === f.value ? "rp-btn--primary" : "rp-btn--outline"}`}
                  style={{ flex: 1 }}
                >
                  {f.icon} {f.label}
                </button>
              ))}
            </div>
          </div>

          <button
            className="rp-btn rp-btn--flood rp-btn--full"
            onClick={handleGenerate}
            disabled={generating}
          >
            {generating ? "Generating..." : "📊 Generate Report"}
          </button>

          <button
            className="rp-btn rp-btn--outline rp-btn--sm"
            onClick={() => navigate("/my-reports")}
            style={{ marginTop: "4px" }}
          >
            ← Back to My Reports
          </button>
        </div>
      </div>
    </div>
  );
}