import { Outlet, Navigate } from "react-router-dom"

const ProtectedRoutes = () => {
    const userToken:string | null = "exampletokenblasdkdfiogjaodf"
    return userToken? <Outlet/> : <Navigate to="/login"/>
}

export default ProtectedRoutes