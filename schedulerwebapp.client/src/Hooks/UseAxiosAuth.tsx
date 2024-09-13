import { useContext, useEffect } from "react";
import { TokenContext } from "../App";
import useRefreshToken from "./UseRefresh";
import {authServer} from "./UseAxios"

const useAxiosAuth = () => {
    const refresh = useRefreshToken()
    const jwt = useContext(TokenContext).jwt
    useEffect(() => {
        const reqIntercept = authServer.interceptors.request.use(
            config => {
                if (!config.headers["Authorization"]) {
                    config.headers["Authorization"] = `Bearer ${jwt}`
                }
                return config
            }, (e) => Promise.reject(e)
        )
        const resIntercept = authServer.interceptors.response.use(
            response => response,
            async (e) => {
                const prevRequest = e.config
                if (e.response.status === 401 && !prevRequest.sent) {
                    prevRequest.sent = true
                    const newJwt = await refresh()
                    prevRequest.headers["Authorization"] = `Bearer ${newJwt}`
                    return authServer(prevRequest)
                }
                return Promise.reject(e);
            }
        )
        return () => {
            authServer.interceptors.request.eject(reqIntercept)
            authServer.interceptors.response.eject(resIntercept)
        }
    }),[jwt, refresh]
    return authServer
}
export default useAxiosAuth