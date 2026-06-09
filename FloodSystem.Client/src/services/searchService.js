import { API_BASE_URL } from "../utils/apiConfig";

export async function searchAlerts(filters) {
  const response = await fetch(`${API_BASE_URL}/Search/alerts`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(filters),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Error searching alerts.");
  return data;
}

export async function searchLocations(filters) {
  const response = await fetch(`${API_BASE_URL}/Search/locations`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(filters),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Error searching locations.");
  return data;
}

export async function searchWeather(filters) {
  const response = await fetch(`${API_BASE_URL}/Search/weather`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(filters),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Error searching weather data.");
  return data;
}

export async function searchTraffic(filters) {
  const response = await fetch(`${API_BASE_URL}/Search/traffic`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(filters),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Error searching traffic updates.");
  return data;
}

export async function searchZones(filters) {
  const response = await fetch(`${API_BASE_URL}/Search/zones`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(filters),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.message || "Error searching zones.");
  return data;
}