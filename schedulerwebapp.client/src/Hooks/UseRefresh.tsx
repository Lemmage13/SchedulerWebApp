import { useContext } from "react"
import { TokenContext } from "../App"
import axios from "axios";

const useRefreshToken = () => {
    const jwtContext = useContext(TokenContext);

    const refresh = async (): Promise<string> => {
        const response = await axios.get("https://localhost:54249/api/Auth/refreshtoken",
            {
                withCredentials: true
            });
        jwtContext.setJwt ? jwtContext.setJwt(response.data.token) : console.error("SET TOKEN UNDEFINED")

        return response.data.token
    }
    return refresh
}
export default useRefreshToken