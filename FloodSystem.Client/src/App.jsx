import { BrowserRouter, Routes, Route, Navigate, useNavigate } from "react-router-dom";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import ProtectedRoute from "./components/auth/ProtectedRoute";
import { useAuth } from "./contexts/AuthContext";
import "./App.css";
import DrainReportPage from "./pages/reports/DrainReportPage";
import FloodReportPage from "./pages/reports/FloodReportPage";
import MyReportsPage from "./pages/reports/MyReportsPage";

function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div style={{ padding: "40px" }}>
      <h1>Dashboard</h1>
      <p>Welcome {user?.fullName}</p>
      <p>Email: {user?.email}</p>
      <p>Role: {user?.roles?.join(", ")}</p>

      <button className="btn btn-danger" onClick={handleLogout}>
        Logout
      </button>
      <div style={{ display: "flex", gap: "10px", marginTop: "20px", flexWrap: "wrap" }}>
  <button onClick={() => navigate("/report/drain")}>🚧 Raporto Kanal</button>
  <button onClick={() => navigate("/report/flood")}>🌊 Raporto Përmbytje</button>
  <button onClick={() => navigate("/my-reports")}>📋 Raportet e Mia</button>
</div>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/report/drain" element={<ProtectedRoute><DrainReportPage /></ProtectedRoute>} />
<Route path="/report/flood" element={<ProtectedRoute><FloodReportPage /></ProtectedRoute>} />
<Route path="/my-reports" element={<ProtectedRoute><MyReportsPage /></ProtectedRoute>} />

        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;