import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { verifyResetCode } from "../../services/authService";
import "./forgotPassword.css";

export default function VerifyResetCode() {
  const navigate = useNavigate();
  const [code, setCode] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      await verifyResetCode(code);
      localStorage.setItem("resetCode", code);
      navigate("/reset-password");
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <div className="auth-page">
      <form className="forgot-card" onSubmit={handleSubmit}>
        <h2>Verify Code</h2>
        <p>Enter the 6-digit code sent to your email.</p>

        {message && <div className="auth-message">{message}</div>}

        <input
          type="text"
          placeholder="Reset code"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          required
        />

        <button type="submit">Verify Code</button>
      </form>
    </div>
  );
}