import axios from "axios";

export const api = axios.create({
    baseURL: "http://localhost:7266/api", // adjust to your backend
    headers: {
        "X-Api-Key": "operator-secret-key",
    },
});