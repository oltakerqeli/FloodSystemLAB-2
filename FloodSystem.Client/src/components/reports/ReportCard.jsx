import "./ReportCard.css";

const STATUS_LABELS = {
  pending: "Në pritje",
  reviewed: "Shqyrtuar",
  resolved: "Zgjidhur",
  active: "Aktive",
};

const SEVERITY_LABELS = {
  low: "E ulët",
  medium: "Mesatare",
  high: "E lartë",
  critical: "Kritike",
};

function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString("sq-AL", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function ReportCard({ report, type = "drain" }) {
  const isFlood = type === "flood";

  return (
    <div className={`report-card report-card--${report.severity}`}>
      <div className="report-card__header">
        <div className="report-card__badges">
          <span className={`badge badge--${report.status}`}>
            {STATUS_LABELS[report.status] ?? report.status}
          </span>
          <span className={`badge badge--${report.severity}`}>
            {SEVERITY_LABELS[report.severity] ?? report.severity}
          </span>
          {isFlood && report.waterLevelCm > 0 && (
            <span className="badge badge--water">
              💧 {report.waterLevelCm} cm
            </span>
          )}
        </div>
        {report.photoUrl && (
          <img
            src={report.photoUrl}
            alt="Foto e raportit"
            className="report-card__photo"
          />
        )}
      </div>

      <p className="report-card__description">{report.description}</p>

      <div className="report-card__footer">
        <span>📍 {report.street}, {report.district}</span>
        <span>🕐 {formatDate(isFlood ? report.reportedAt : report.createdAt)}</span>
      </div>
    </div>
  );
}