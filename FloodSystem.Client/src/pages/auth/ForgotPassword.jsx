import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { forgotPassword } from "../../services/authService";
import "./forgotPassword.css";

export default function ForgotPassword() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            await forgotPassword(email);
            setMessage("If this email exists, a reset code has been sent.");
            setTimeout(() => navigate("/verify-reset-code"), 1000);
        } catch (error) {
            setMessage(error.message);
        }
    };

    return (
        <div className="auth-page">
            <form className="forgot-card" onSubmit={handleSubmit}>
                <h2>Forgot Password</h2>
                <p>Enter your email to receive a reset code.</p>

                {message && <div className="auth-message">{message}</div>}

                <input
                    type="email"
                    placeholder="Email address"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />

                <button type="submit">
                    Send Code
                </button>

                <p
                    className="forgot-link"
                    onClick={() => navigate("/login")}
                >
                    Back to Login
                </p>
            </form>
        </div>
    );
}