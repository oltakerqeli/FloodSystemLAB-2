import { useEffect, useState } from "react";
import { getUsers, deactivateUser, activateUser, assignRole } from "../../services/usersApi";
import "./ManageUsers.css";
import { useNavigate } from "react-router-dom";


export default function ManageUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const loadUsers = async () => {
    try {
      setLoading(true);
      const data = await getUsers();
      setUsers(data);
    } catch (error) {
      setMessage(error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

const handleDeactivate = async (id) => {
  try {
    await deactivateUser(id);
    setMessage("User deactivated successfully.");
    await loadUsers();
  } catch (error) {
    setMessage(error.message);
  }
};

const handleActivate = async (id) => {
  try {
    await activateUser(id);
    setMessage("User activated successfully.");
    await loadUsers();
  } catch (error) {
    setMessage(error.message);
  }
};

  const handleAssignRole = async (id, roleName) => {
    try {
      await assignRole(id, roleName);
      setMessage(`Role changed to ${roleName}.`);
      await loadUsers();
    } catch (error) {
      setMessage(error.message);
    }
  };

  if (loading) return <p>Loading users...</p>;

  return (
    <div className="manage-users-page">
      <div className="manage-users-container">

        <div className="manage-users-header">
          <h1>Manage Users</h1>

          <button
            className="back-btn"
            onClick={() => navigate("/admin")}
          >
            ← Back
          </button>
        </div>

        {message && (
          <div className="manage-users-message">
            {message}
          </div>
        )}

        <div className="manage-users-table-wrapper">
          <table className="manage-users-table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Full Name</th>
                <th>Email</th>
                <th>Active</th>
                <th>Roles</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.id}</td>

                  <td>
                    {user.firstName} {user.lastName}
                  </td>

                  <td>{user.email}</td>

                  <td>
                    {user.isActive ? "✅ Yes" : "❌ No"}
                  </td>

                  <td>
                    {user.roles?.length > 0
                      ? user.roles.join(", ")
                      : "No Role"}
                  </td>

                  <td>
                    {user.isActive ? (
                      <button
                        className="btn-action btn-deactivate"
                        onClick={() => handleDeactivate(user.id)}
                      >
                        Deactivate
                      </button>
                    ) : (
                      <button
                        className="btn-action btn-activate"
                        onClick={() => handleActivate(user.id)}
                      >
                        Activate
                      </button>
                    )}

                    <select
                      className="role-select"
                      value={user.roles?.[0] || ""}
                      onChange={(e) => handleAssignRole(user.id, e.target.value)}
                    >
                      <option value="" disabled>
                        Select role
                      </option>
                      <option value="User">User</option>
                      <option value="Authority">Authority</option>
                      <option value="Admin">Admin</option>
                    </select>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

      </div>
    </div>
  );
}