import { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useCreateDrainReport } from "../../hooks/useReports";
import { FaMapMarkerAlt, FaAlignLeft, FaUser, FaCamera, FaTimes, FaExclamationTriangle, FaCheckCircle } from "react-icons/fa";
import "./ReportPage.css";

const DISTRICTS = [
  "Center", "Dardania", "Arberia/Dragodan", "Bregu i Diellit",
  "Kalabria", "Matiqan", "Tophane", "Lakrishte", "Fushe Kosove", "Gracanice", "Aktash", "Upliane",
];

export default function DrainReportPage() {
  const navigate = useNavigate();
  const { submit, loading } = useCreateDrainReport();
  const fileInputRef = useRef(null);

  const [submitted, setSubmitted] = useState(false);
  const [photoPreview, setPhotoPreview] = useState(null);
  const [photoFile, setPhotoFile] = useState(null);
  const [errors, setErrors] = useState({});
  const [form, setForm] = useState({
    reporterName: "", street: "", district: "", description: "",
    severity: "medium", lat: 42.6629, lng: 21.1655,
  });

  const validate = () => {
    const e = {};
    if (!form.street.trim() || form.street.length < 2) e.street = "Street is required";
    if (!form.district) e.district = "District is required";
    if (!form.description.trim() || form.description.length < 10) e.description = "Description must be at least 10 characters";
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    setErrors((prev) => ({ ...prev, [e.target.name]: undefined }));
  };

  const handlePhoto = (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (ev) => setPhotoPreview(ev.target.result);
    reader.readAsDataURL(file);
    setPhotoFile(file);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validate()) return;
    submit(
      { ...form, photo: photoFile },
      {
        onSuccess: () => setSubmitted(true),
        onError: () => alert("Submission failed. Please try again."),
      }
    );
  };

  if (submitted) {
    return (
      <div className="rp-page">
        <div className="rp-card">
          <div className="rp-success-icon"><FaCheckCircle /></div>
          <h2 className="rp-success-title">Report Submitted!</h2>
          <p className="rp-success-msg">Your drain blockage report has been sent to the maintenance team.</p>
          <div className="rp-success-actions">
            <button className="rp-btn rp-btn--outline" onClick={() => {
              setSubmitted(false);
              setForm({ reporterName: "", street: "", district: "", description: "", severity: "medium", lat: 42.6629, lng: 21.1655 });
              setPhotoPreview(null);
              setPhotoFile(null);
            }}>
              Submit another
            </button>
            <button className="rp-btn rp-btn--primary" onClick={() => navigate("/my-reports")}>
              My reports
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="rp-page">
      <div className="rp-card">
        <div className="rp-header">
          <div className="rp-bubble rp-bubble--drain">🚧</div>
          <div>
            <h1 className="rp-title">Report Blocked Drain</h1>
            <p className="rp-subtitle">Help keep the city safe by reporting drain blockages</p>
          </div>
        </div>
        <button type="button" className="rp-btn rp-btn--outline rp-btn--full" onClick={() => navigate("/my-reports")}>
          ← Back to My Reports
        </button>

        <div className="rp-divider" />

        <form onSubmit={handleSubmit} className="rp-form">

          <div className="rp-field-group">
            <label className="rp-label">Your name (optional)</label>
            <div className="rp-field">
              <div className="rp-field-icon"><FaUser /></div>
              <input name="reporterName" value={form.reporterName} onChange={handleChange} placeholder="Anonymous" />
            </div>
          </div>

          <div className="rp-row">
            <div className="rp-field-group">
              <label className="rp-label">Street / Location</label>
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

          <div className="rp-field-group">
            <label className="rp-label">Severity</label>
            <div className="rp-field">
              <div className="rp-field-icon"><FaExclamationTriangle /></div>
              <select name="severity" value={form.severity} onChange={handleChange}>
                <option value="low">🟢 Low</option>
                <option value="medium">🟡 Medium</option>
                <option value="high">🔴 High</option>
              </select>
            </div>
          </div>

          <div className="rp-field-group">
            <label className="rp-label">Description</label>
            <div className={`rp-field rp-field--textarea ${errors.description ? "rp-field--error" : ""}`}>
              <div className="rp-field-icon" style={{ marginTop: "2px" }}><FaAlignLeft /></div>
              <textarea name="description" value={form.description} onChange={handleChange} placeholder="Describe the drain blockage problem..." rows={4} />
            </div>
            {errors.description && <span className="rp-error">{errors.description}</span>}
          </div>

          <div className="rp-field-group">
            <label className="rp-label">Photo (optional)</label>
            <input ref={fileInputRef} type="file" accept="image/*" onChange={handlePhoto} style={{ display: "none" }} />
            {photoPreview ? (
              <div className="rp-photo-preview">
                <img src={photoPreview} alt="Preview" />
                <button type="button" className="rp-photo-remove" onClick={() => {
                  setPhotoPreview(null);
                  setPhotoFile(null);
                  if (fileInputRef.current) fileInputRef.current.value = "";
                }}>
                  <FaTimes /> Remove photo
                </button>
              </div>
            ) : (
              <button type="button" className="rp-photo-btn" onClick={() => fileInputRef.current?.click()}>
                <div className="rp-bubble rp-bubble--camera"><FaCamera /></div>
                <span>Add photo</span>
                <small>JPG, PNG up to 10MB</small>
              </button>
            )}
          </div>
          <button type="submit" className="rp-btn rp-btn--primary rp-btn--full" disabled={loading}>
          {loading ? "Submitting..." : "🚧 Submit Report"}
        </button>
        </form>
      </div>
    </div>
  );
}