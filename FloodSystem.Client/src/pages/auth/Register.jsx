import { useState } from "react";
import {
    FaEnvelope,
    FaLock,
    FaUser,
    FaEye,
    FaEyeSlash,
} from "react-icons/fa";
import { registerUser } from "../../services/authService";
import "./Auth.css";

function Register() {
    const [formData, setFormData] = useState({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        confirmPassword: "",
    });

    const [message, setMessage] = useState("");
    const [isError, setIsError] = useState(false);

    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage("");
        setIsError(false);

        if (formData.password !== formData.confirmPassword) {
            setIsError(true);
            setMessage("Passwords do not match.");
            return;
        }

        try {
            const payload = {
                firstName: formData.firstName,
                lastName: formData.lastName,
                email: formData.email,
                password: formData.password,
            };

            const result = await registerUser(payload);
            setMessage(result.message || "Registration successful.");

            setTimeout(() => {
                window.location.href = "/login";
            }, 1000);
        } catch (error) {
            setIsError(true);
            setMessage(error.message);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-card register-card">
                <h1>Flood System</h1>
                <p>Create your account</p>

                {message && (
                    <div className={`alert ${isError ? "alert-danger" : "alert-success"}`}>
                        {message}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="auth-field">
                        <FaUser />
                        <input name="firstName" placeholder="First Name" value={formData.firstName} onChange={handleChange} required />
                    </div>

                    <div className="auth-field">
                        <FaUser />
                        <input name="lastName" placeholder="Last Name" value={formData.lastName} onChange={handleChange} required />
                    </div>

                    <div className="auth-field">
                        <FaEnvelope />
                        <input type="email" name="email" placeholder="Email address" value={formData.email} onChange={handleChange} required />
                    </div>

                    <div className="password-row">
                        <div className="auth-field">
                            <FaLock />
                            <input type={showPassword ? "text" : "password"} name="password" placeholder="Password" value={formData.password} onChange={handleChange} required />
                            <span
                                className="password-toggle"
                                onClick={() => setShowPassword(!showPassword)}
                            >
                                {showPassword ? <FaEyeSlash /> : <FaEye />}
                            </span>
                        </div>

                        <div className="auth-field">
                            <FaLock />
                            <input type={showConfirmPassword ? "text" : "password"} name="confirmPassword" placeholder="Confirm Password" value={formData.confirmPassword} onChange={handleChange} required />
                            <span
                                className="password-toggle"
                                onClick={() =>
                                    setShowConfirmPassword(!showConfirmPassword)
                                }
                                >
                                {showConfirmPassword ? <FaEyeSlash /> : <FaEye />}
                            </span>
                        </div>
                    </div>

                    <button type="submit" className="auth-btn">Create Account</button>
                </form>

                <span className="auth-link">
                    Already have an account? <a href="/login">Sign In</a>
                </span>
            </div>
        </div>
    );
}

export default Register;