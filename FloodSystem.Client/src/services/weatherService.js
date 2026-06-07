import { API_BASE_URL } from "../utils/apiConfig";

async function fetchApi(endpoint, options = {}) {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });
  
  // Nëse statusi është 204 (No Content), kthen null
  if (response.status === 204) {
    return null;
  }
  
  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message || `HTTP ${response.status}`);
  }
  
  return response.json();
}

// ==================== LOCATIONS ====================
export const getLocations = () => fetchApi("/Locations");
export const getLocationById = (id) => fetchApi(`/Locations/${id}`);
export const createLocation = (data) => fetchApi("/Locations", { method: "POST", body: JSON.stringify(data) });
export const updateLocation = (id, data) => fetchApi(`/Locations/${id}`, { method: "PUT", body: JSON.stringify(data) });
export const deleteLocation = (id) => fetchApi(`/Locations/${id}`, { method: "DELETE" });

// ==================== ZONES ====================
export const getZones = () => fetchApi("/Zones");
export const getZoneById = (id) => fetchApi(`/Zones/${id}`);
export const createZone = (data) => fetchApi("/Zones", { method: "POST", body: JSON.stringify(data) });
export const updateZone = (id, data) => fetchApi(`/Zones/${id}`, { method: "PUT", body: JSON.stringify(data) });
export const deleteZone = (id) => fetchApi(`/Zones/${id}`, { method: "DELETE" });

// ==================== ALERTS ====================
export const getAlerts = () => fetchApi("/Alerts");
export const getActiveAlerts = () => fetchApi("/Alerts/active");
export const getAlertsByLocation = (locationId) => fetchApi(`/Alerts/location/${locationId}`);
export const getAlertById = (id) => fetchApi(`/Alerts/${id}`);

// ==================== WEATHER ====================
export const getWeatherLatest = (locationId) => fetchApi(`/Weather/${locationId}/latest`);

// ✅ FUNKSIONI I RI - EKSPORTOJE KËTË
export const getWeatherLatestAll = async () => {
  const locations = await getLocations();
  const weatherPromises = locations.map(async (loc) => {
    try {
      return await fetchApi(`/Weather/${loc.id}/latest`);
    } catch {
      return null;
    }
  });
  const results = await Promise.all(weatherPromises);
  return results.filter(r => r !== null);
};

export const getWeatherHistory = (locationId) => fetchApi(`/Weather/${locationId}/history`);
export const getRiskLevel = (locationId) => fetchApi(`/Weather/${locationId}/risk`);
export const refreshWeather = (locationId) => fetchApi(`/Weather/fetch/${locationId}`, { method: "POST" });
export const refreshAllWeather = () => fetchApi("/Weather/fetch-all", { method: "POST" });

// ==================== TRAFFIC ====================
export const getSafeRoutes = () => fetchApi("/Traffic/safe-routes");
export const getLatestTraffic = (locationId) => fetchApi(`/Traffic/location/${locationId}/latest`);
export const getTrafficHistory = (locationId) => fetchApi(`/Traffic/location/${locationId}/history`);