const API_URL = "http://localhost:5164/api";

export const getUsers = async () => {
  const res = await fetch(`${API_URL}/Users`, {
    method: "GET",
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to load users");
  return await res.json();
};

export const deactivateUser = async (id) => {
  const res = await fetch(`${API_URL}/Users/${id}/deactivate`, {
    method: "PUT",
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to deactivate user");
};

export const assignRole = async (id, roleName) => {
  const res = await fetch(`${API_URL}/Users/${id}/assign-role`, {
    method: "PUT",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ roleName: roleName }),
  });

  if (!res.ok) {
    const error = await res.json();
    throw new Error(error.message || "Failed to assign role");
  }

  return await res.json();
};

export const activateUser = async (id) => {
  const res = await fetch(`${API_URL}/Users/${id}/activate`, {
    method: "PUT",
    credentials: "include",
  });

  if (!res.ok) {
    const error = await res.json();
    throw new Error(error.message || "Failed to activate user");
  }

  return await res.json();
};