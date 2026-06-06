import { API_BASE_URL } from "../utils/apiConfig";

export async function getDrainReports(status = null) {
  const url = status
    ? `${API_BASE_URL}/Reports?status=${status}`
    : `${API_BASE_URL}/Reports`;
  const response = await fetch(url, {
    method: "GET",
    credentials: "include",
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë marrjes së raporteve.");
  return data;
}

export async function createDrainReport(reportData) {
  const response = await fetch(`${API_BASE_URL}/Reports`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(reportData),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë dërgimit të raportit.");
  return data;
}

export async function getFloodReports(params = {}) {
  const query = new URLSearchParams(params).toString();
  const url = query
    ? `${API_BASE_URL}/FloodReports?${query}`
    : `${API_BASE_URL}/FloodReports`;
  const response = await fetch(url, {
    method: "GET",
    credentials: "include",
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Gabim gjatë marrjes së raporteve.");
  return data;
}

export async function createFloodReport(reportData) {
  const response = await fetch(`${API_BASE_URL}/FloodReports`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(reportData),
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