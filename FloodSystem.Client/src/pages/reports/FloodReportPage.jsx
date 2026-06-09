import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCreateFloodReport } from "../../hooks/useReports";
import { FaMapMarkerAlt, FaAlignLeft, FaTint, FaExclamationTriangle, FaCheckCircle } from "react-icons/fa";
import "./ReportPage.css";

const DISTRICTS = [
  "Center", "Dardania", "Arberia/Dragodan", "Bregu i Diellit",
  "Kalabria", "Matiqan", "Tophane", "Lakrishte", "Fushe Kosove", "Gracanice", "Aktash", "Upliane",
];

export default function FloodReportPage() {
  const navigate = useNavigate();
  const { submit, loading } = useCreateFloodReport();

  const [submitted, setSubmitted] = useState(false);
  const [errors, setErrors] = useState({});
  const [form, setForm] = useState({
    location: "", street: "", district: "", description: "",
    severity: "medium", waterLevelCm: 0, lat: 42.6629, lng: 21.1655,
  });

  const validate = () => {
    const e = {};
    if (!form.location.trim() || form.location.length < 2) e.location = "Location is required";
    if (!form.street.trim() || form.street.length < 2) e.street = "Street is required";
    if (!form.district) e.district = "District is required";
    if (!form.description.trim() || form.description.length < 10) e.description = "Description must be at least 10 characters";
    if (form.waterLevelCm < 0 || form.waterLevelCm > 500) e.waterLevelCm = "Water level must be between 0-500 cm";
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleChange = (e) => {
    const val = e.target.name === "waterLevelCm" ? Number(e.target.value) : e.target.value;
    setForm((prev) => ({ ...prev, [e.target.name]: val }));
    setErrors((prev) => ({ ...prev, [e.target.name]: undefined }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validate()) return;
    submit(form, {
      onSuccess: () => setSubmitted(true),
      onError: () => alert("Submission failed. Please try again."),
    });
  };

  if (submitted) {
    return (
      <div className="rp-page">
        <div className="rp-card">
          <div className="rp-success-icon"><FaCheckCircle /></div>
          <h2 className="rp-success-title">Report Submitted!</h2>
          <p className="rp-success-msg">Your flood report has been sent to the relevant authorities.</p>
          <div className="rp-success-actions">
            <button className="rp-btn rp-btn--outline" onClick={() => {
              setSubmitted(false);
              setForm({ location: "", street: "", district: "", description: "", severity: "medium", waterLevelCm: 0, lat: 42.6629, lng: 21.1655 });
            }}>Submit another</button>
            <button className="rp-btn rp-btn--flood" onClick={() => navigate("/my-reports")}>My reports</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="rp-page">
      <div className="rp-card">
        <div className="rp-header">
          <div className="rp-bubble rp-bubble--flood">🌊</div>
          <div>
            <h1 className="rp-title">Report Flood</h1>
            <p className="rp-subtitle">Report a flooding incident in your area</p>
          </div>
        </div>
        <button type="button" className="rp-btn rp-btn--outline rp-btn--full" onClick={() => navigate("/my-reports")}>
            ← Back to My Reports
          </button>

        <div className="rp-divider" />

        <form onSubmit={handleSubmit} className="rp-form">

          <div className="rp-field-group">
            <label className="rp-label">Location</label>
            <div className={`rp-field ${errors.location ? "rp-field--error" : ""}`}>
              <div className="rp-field-icon"><FaMapMarkerAlt /></div>
              <input name="location" value={form.location} onChange={handleChange} placeholder="e.g. City Park" />
            </div>
            {errors.location && <span className="rp-error">{errors.location}</span>}
          </div>

          <div className="rp-row">
            <div className="rp-field-group">
              <label className="rp-label">Street</label>
              <div className={`rp-field ${errors.street ? "rp-field--error" : ""}`}>
                <div className="rp-field-icon"><FaMapMarkerAlt /></div>
                <input name="street" value={form.street} onChange={handleChange} placeholder="e.g. UCK St., No. 45" />
              </div>
              {errors.street && <span className="rp-error">{errors.street}</span>}
            </div>

            <div className="rp-field-group">
              <label className="rp-label">District</label>
              <div className={`rp-field ${errors.district ? "rp-field--error" : ""}`}>
                <div className="rp-field-icon"><FaMapMarkerAlt /></div>
                <select name="district" value={form.district} onChange={handleChange}>
                  <option value="">Select district</option>
                  {DISTRICTS.map((d) => <option key={d} value={d}>{d}</option>)}
                </select>
              </div>
              {errors.district && <span className="rp-error">{errors.district}</span>}
            </div>
          </div>

          <div className="rp-row">
            <div className="rp-field-group">
              <label className="rp-label">Risk level</label>
              <div className="rp-field">
                <div className="rp-field-icon"><FaExclamationTriangle /></div>
                <select name="severity" value={form.severity} onChange={handleChange}>
                  <option value="low">🟢 Low</option>
                  <option value="medium">🟡 Medium</option>
                  <option value="high">🟠 High</option>
                  <option value="critical">🔴 Critical</option>
                </select>
              </div>
            </div>

            <div className="rp-field-group">
              <label className="rp-label">Water level (cm)</label>
              <div className={`rp-field ${errors.waterLevelCm ? "rp-field--error" : ""}`}>
                <div className="rp-field-icon"><FaTint /></div>
                <input type="number" name="waterLevelCm" value={form.waterLevelCm} onChange={handleChange} min={0} max={500} />
              </div>
              {errors.waterLevelCm && <span className="rp-error">{errors.waterLevelCm}</span>}
            </div>
          </div>

          <div className="rp-field-group">
            <label className="rp-label">Description</label>
            <div className={`rp-field rp-field--textarea ${errors.description ? "rp-field--error" : ""}`}>
              <div className="rp-field-icon" style={{ marginTop: "2px" }}><FaAlignLeft /></div>
              <textarea name="description" value={form.description} onChange={handleChange} placeholder="Describe the flooding situation..." rows={4} />
            </div>
            {errors.description && <span className="rp-error">{errors.description}</span>}
          </div>

          <button type="submit" className="rp-btn rp-btn--flood rp-btn--full" disabled={loading}>
            {loading ? "Submitting..." : "🌊 Submit Flood Report"}
          </button>
        </form>
      </div>
    </div>
  );
}