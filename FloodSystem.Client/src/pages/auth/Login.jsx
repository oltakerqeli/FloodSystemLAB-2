import { useState } from "react";
import {
  FaEnvelope, FaLock, FaEye,
  FaEyeSlash,
} from "react-icons/fa";
import { loginUser } from "../../services/authService";
import "./Auth.css";

function Login() {
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");
    setIsError(false);

    try {
      const result = await loginUser(formData);
      localStorage.setItem("accessToken", result.accessToken);
      localStorage.setItem("refreshToken", result.refreshToken);
      window.location.href = "/dashboard";
    } catch (error) {
      setIsError(true);
      setMessage(error.message);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <h1>Flood System</h1>
        <p>Sign in to continue</p>

        {message && (
          <div className={`alert ${isError ? "alert-danger" : "alert-success"}`}>
            {message}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="auth-field">
            <FaEnvelope />
            <input
              type="email"
              name="email"
              placeholder="Email address"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <div className="auth-field">
            <FaLock />

            <input
              type={showPassword ? "text" : "password"}
              name="password"
              placeholder="Password"
              value={formData.password}
              onChange={handleChange}
              required
            />

            <span
              className="password-toggle"
              onClick={() => setShowPassword(!showPassword)}
            >
              {showPassword ? <FaEyeSlash /> : <FaEye />}
            </span>
          </div>

          <button type="submit" className="auth-btn">Sign In</button>
        </form>

        <span className="auth-link">
          Don&apos;t have an account? <a href="/register">Create Account</a>
        </span>
      </div>
    </div>
  );
}

export default Login;