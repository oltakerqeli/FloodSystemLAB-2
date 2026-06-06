import { API_BASE_URL } from "../utils/apiConfig";

export async function getDrainReports() {
  const response = await fetch(`${API_BASE_URL}/Reports/drain`, {
    method: "GET",
    credentials: "include",
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë marrjes së raporteve.");
  return data;
}

export async function createDrainReport(reportData) {
  const formData = new FormData();
  formData.append("locationId", reportData.locationId || 1);
  formData.append("description", reportData.description);
  formData.append("street", reportData.street || "");
  formData.append("district", reportData.district || "");
  formData.append("severity", reportData.severity || "medium");
  if (reportData.reporterName) formData.append("reporterName", reportData.reporterName);
  if (reportData.photo instanceof File) formData.append("photo", reportData.photo);

  const response = await fetch(`${API_BASE_URL}/Reports/drain`, {
    method: "POST",
    credentials: "include",
    body: formData,
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë dërgimit të raportit.");
  return data;
}

export async function getFloodReports() {
  const response = await fetch(`${API_BASE_URL}/Reports/flood`, {
    method: "GET",
    credentials: "include",
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë marrjes së raporteve.");
  return data;
}

export async function createFloodReport(reportData) {
  const response = await fetch(`${API_BASE_URL}/Reports/flood`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({
      locationId: reportData.locationId || 1,
      description: reportData.description,
      street: reportData.street || "",
      district: reportData.district || "",
      severity: reportData.severity || "medium",
      locationName: reportData.location || "",
      waterLevelCm: reportData.waterLevelCm || 0,
    }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë dërgimit të raportit.");
  return data;
}

export async function uploadPhoto(file) {
  const formData = new FormData();
  formData.append("photo", file);
  const response = await fetch(`${API_BASE_URL}/Files/upload`, {
    method: "POST",
    credentials: "include",
    body: formData,
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë ngarkimit të fotos.");
  return data;
}