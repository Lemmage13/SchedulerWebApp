import { AxiosInstance } from "axios";
import { useContext, useEffect } from "react";
import { TokenContext } from "../App";
const useAxiosAuth = (instance: AxiosInstance) => {
    const tokenContext = useContext(TokenContext)
    useEffect(() => {
        const reqIntercept = instance.interceptors.request.use(
            config => {
                if (!config.headers.Authorization) {
                    config.headers.Authorization = `Bearer ${tokenContext.token}`
                }
                return config
            }, (e) => {
                console.error(e)
            }
        )
        return () => {
            instance.interceptors.request.eject(reqIntercept)
        }
    })
    return instance
}
export default useAxiosAuth