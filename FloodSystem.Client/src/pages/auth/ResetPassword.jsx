import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { resetPassword } from "../../services/authService";
import "./forgotPassword.css";

export default function ResetPassword() {
    const navigate = useNavigate();
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        const code = localStorage.getItem("resetCode");

        if (!code) {
            setMessage("Reset code not found. Please request a new code.");
            return;
        }

        try {
            await resetPassword(code, newPassword, confirmPassword);
            localStorage.removeItem("resetCode");
            setMessage("Password reset successfully.");
            setTimeout(() => navigate("/login"), 1000);
        } catch (error) {
            setMessage(error.message);
        }
    };

    

    return (
        <div className="auth-page">
            <form className="forgot-card" onSubmit={handleSubmit}>
                <h2>Reset Password</h2>

                {message && <div className="auth-message">{message}</div>}

                <input
                    type="password"
                    placeholder="New password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    required
                />

                <input
                    type="password"
                    placeholder="Confirm password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />

                <button type="submit">
                    Reset Password
                </button>
            </form>
        </div>
    );
}