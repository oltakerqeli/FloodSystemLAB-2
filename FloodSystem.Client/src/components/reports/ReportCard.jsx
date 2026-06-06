import { useState } from "react";
import "./ReportCard.css";

const STATUS_LABELS = {
  pending: "Pending",
  reviewed: "Reviewed",
  resolved: "Resolved",
  active: "Active",
};

const SEVERITY_LABELS = {
  low: "Low",
  medium: "Medium",
  high: "High",
  critical: "Critical",
};

function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString("en-GB", {
    day: "2-digit", month: "short", year: "numeric",
    hour: "2-digit", minute: "2-digit",
  });
}

function getPhotoUrl(photoUrl) {
  if (!photoUrl) return null;
  if (photoUrl.startsWith("http")) return photoUrl;
  const filename = photoUrl.split("\\").pop().split("/").pop();
  return `http://localhost:5164/uploads/${filename}`;
}

function ReportModal({ report, type, onClose }) {
  const isFlood = type === "flood";
  const photo = getPhotoUrl(report.photoUrl);

  return (
    <div className="rc-modal-overlay" onClick={onClose}>
      <div className="rc-modal" onClick={e => e.stopPropagation()}>
        <div className="rc-modal__header">
          <div>
            <h2 className="rc-modal__title">
              {isFlood ? "🌊 Flood Report" : "🚧 Drain Report"}
            </h2>
            <span className="rc-modal__date">📅 {formatDate(report.createdAt)}</span>
          </div>
          <button className="rc-modal__close" onClick={onClose}>✕</button>
        </div>

        <div className="rc-modal__body">
          <div className="rc-modal__badges">
            <span className={`badge badge--${report.status?.toLowerCase()}`}>
              {STATUS_LABELS[report.status?.toLowerCase()] ?? report.status}
            </span>
            <span className={`badge badge--${report.severity?.toLowerCase()}`}>
              {SEVERITY_LABELS[report.severity?.toLowerCase()] ?? report.severity}
            </span>
            {isFlood && report.waterLevelCm > 0 && (
              <span className="badge badge--water">💧 {report.waterLevelCm} cm</span>
            )}
          </div>

          {photo && (
            <img src={photo} alt="Report" className="rc-modal__photo" />
          )}

          <div className="rc-modal__section">
            <label>Description</label>
            <p>{report.description}</p>
          </div>

          <div className="rc-modal__grid">
            {report.street && (
              <div className="rc-modal__section">
                <label>Street</label>
                <p>📍 {report.street}</p>
              </div>
            )}
            {report.district && (
              <div className="rc-modal__section">
                <label>District</label>
                <p>🏘 {report.district}</p>
              </div>
            )}
            {isFlood && report.locationName && (
              <div className="rc-modal__section">
                <label>Location</label>
                <p>📌 {report.locationName}</p>
              </div>
            )}
            {!isFlood && report.reporterName && (
              <div className="rc-modal__section">
                <label>Reported by</label>
                <p>👤 {report.reporterName}</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default function ReportCard({ report, type = "drain" }) {
  const [showModal, setShowModal] = useState(false);
  const isFlood = type === "flood";
  const photo = getPhotoUrl(report.photoUrl);

  return (
    <>
      <div
        className={`report-card report-card--${report.severity?.toLowerCase()}`}
        onClick={() => setShowModal(true)}
        style={{ cursor: "pointer" }}
      >
        <div className="report-card__header">
          <div className="report-card__badges">
            <span className={`badge badge--${report.status?.toLowerCase()}`}>
              {STATUS_LABELS[report.status?.toLowerCase()] ?? report.status}
            </span>
            <span className={`badge badge--${report.severity?.toLowerCase()}`}>
              {SEVERITY_LABELS[report.severity?.toLowerCase()] ?? report.severity}
            </span>
            {isFlood && report.waterLevelCm > 0 && (
              <span className="badge badge--water">
                💧 {report.waterLevelCm} cm
              </span>
            )}
          </div>
          {photo && (
            <img src={photo} alt="Report photo" className="report-card__photo" />
          )}
        </div>

        <p className="report-card__description">{report.description}</p>

        <div className="report-card__footer">
          <span>📍 {report.street}{report.district ? `, ${report.district}` : ""}</span>
          <span>🕐 {formatDate(report.createdAt)}</span>
        </div>
      </div>

      {showModal && (
        <ReportModal report={report} type={type} onClose={() => setShowModal(false)} />
      )}
    </>
  );
}