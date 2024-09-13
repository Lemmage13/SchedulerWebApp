import axios from "axios"

const baseUrl = "https://localhost:54249/api/"
export const server =  axios.create({
    baseURL: baseUrl,
    withCredentials: true
})

export const authServer = axios.create({
    baseURL: baseUrl,
    headers: { "Content-Type": "application/json" },
    withCredentials: true
});