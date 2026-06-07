import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getWeatherLatestAll, refreshAllWeather } from "../../services/weatherService";
import "./WeatherPage.css";

export default function WeatherPage() {
  const navigate = useNavigate();
  const [weather, setWeather] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getWeatherLatestAll();
      setWeather(data || []);
    } catch (error) {
      console.error("Error loading weather:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
    // Refresh every 30 seconds (ose sa të duash)
    const interval = setInterval(loadData, 30000);
    return () => clearInterval(interval);
  }, [loadData]);

  const handleRefresh = async () => {
    setRefreshing(true);
    try {
      await refreshAllWeather();
      await loadData();
      alert("Weather data refreshed");
    } catch (error) {
      alert("Failed to refresh");
    } finally {
      setRefreshing(false);
    }
  };

  if (loading) return <div className="wp-page"><div className="wp-container">Loading weather data...</div></div>;

  return (
    <div className="wp-page">
      <div className="wp-container">
        <div className="wp-header">
          <div className="wp-header__left">
            <div className="wp-header__icon">🌤️</div>
            <div><h1>Weather & DSS</h1><p>Real-time weather data from OpenWeatherMap</p></div>
          </div>
          <div className="wp-header__actions">
            <button className="wp-btn wp-btn--outline" onClick={() => navigate("/dashboard")}>← Back</button>
            <button className="wp-btn wp-btn--primary" onClick={handleRefresh} disabled={refreshing}>
              {refreshing ? "Refreshing..." : "Refresh All"}
            </button>
          </div>
        </div>

        <div className="wp-weather-grid">
          {weather.length === 0 ? (
            <div className="wp-empty"><div className="wp-empty__icon">🌤️</div><p>No weather data available. Click Refresh to fetch data from OpenWeatherMap.</p></div>
          ) : (
            weather.map((w) => (
              <div key={w.id} className="wp-weather-card">
                <div className="wp-weather-card__top">
                  <div><p className="wp-weather-card__loc">{w.locationName}</p><p className="wp-weather-card__time">{new Date(w.recordedAt).toLocaleString()}</p></div>
                  <div className="wp-weather-card__icon">🌤️</div>
                </div>
                <div className="wp-weather-card__temp">{w.temperature}°C</div>
                <div className="wp-weather-card__stats">
                  <div className="wp-stat"><span className="wp-stat__label">💧 Humidity</span><span className="wp-stat__val">{w.humidity}%</span></div>
                  <div className="wp-stat"><span className="wp-stat__label">🌧️ Rainfall</span><span className="wp-stat__val">{w.rainfall} mm</span></div>
                </div>
                <div className={`wp-risk wp-risk--${w.riskLevel?.toLowerCase() || "low"}`}>
                  {w.riskLevel === "HIGH" ? "🔴 HIGH RISK" : w.riskLevel === "MEDIUM" ? "🟡 MEDIUM RISK" : "🟢 LOW RISK"}
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
}