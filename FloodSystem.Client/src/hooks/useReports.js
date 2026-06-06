import { useState, useEffect, useCallback } from "react";
import {
  getDrainReports,
  getFloodReports,
  createDrainReport,
  createFloodReport,
  uploadPhoto,
} from "../services/reportService";

export function useDrainReports(statusFilter = null) {
  const [reports, setReports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchReports = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getDrainReports(statusFilter);
      setReports(data || []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [statusFilter]);

  useEffect(() => { fetchReports(); }, [fetchReports]);

  return { reports, loading, error, refetch: fetchReports };
}

export function useFloodReports(params = {}) {
  const [reports, setReports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchReports = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getFloodReports(params);
      setReports(data || []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [JSON.stringify(params)]);

  useEffect(() => { fetchReports(); }, [fetchReports]);

  return { reports, loading, error, refetch: fetchReports };
}

export function useCreateDrainReport() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const submit = async (data, { onSuccess, onError } = {}) => {
    try {
      setLoading(true);
      setError(null);
      const result = await createDrainReport(data);
      onSuccess?.(result);
    } catch (err) {
      setError(err.message);
      onError?.(err);
    } finally {
      setLoading(false);
    }
  };

  return { submit, loading, error };
}

export function useCreateFloodReport() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const submit = async (data, { onSuccess, onError } = {}) => {
    try {
      setLoading(true);
      setError(null);
      const result = await createFloodReport(data);
      onSuccess?.(result);
    } catch (err) {
      setError(err.message);
      onError?.(err);
    } finally {
      setLoading(false);
    }
  };

  return { submit, loading, error };
}

export function usePhotoUpload() {
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState(null);

  const upload = async (file) => {
    try {
      setUploading(true);
      setError(null);
      const result = await uploadPhoto(file);
      return result?.url || null;
    } catch (err) {
      setError(err.message);
      return null;
    } finally {
      setUploading(false);
    }
  };

  return { upload, uploading, error };
}