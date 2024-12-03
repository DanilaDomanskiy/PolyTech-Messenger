import { useNavigate } from "react-router-dom";

export function useHandleError() {
  const navigate = useNavigate();

  return (error, setError) => {
    if (error?.response?.status === 401 || error?.response?.status === 403) {
      navigate("/login");
    } else {
      setError(true);
      console.error(error);
    }
  };
}
