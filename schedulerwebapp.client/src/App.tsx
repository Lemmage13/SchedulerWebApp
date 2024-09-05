import Login from './Login';
import WeekPlanner from './WeekPlanner'
import { BrowserRouter, Routes, Route } from "react-router-dom"
import ProtectedRoutes from "./Tools/ProtectedRoutes"
import Register from './Register';
import { createContext, useState } from 'react';
import ITokenContext from './interfaces/TokenContext';

export const TokenContext = createContext<ITokenContext>({})
function App() {
    const [token, setToken] = useState(null)
    const tokenContextValue: ITokenContext = {token, setToken}

    return (
        <TokenContext.Provider value={tokenContextValue}>
            <BrowserRouter> 
                <Routes>
                    <Route element={<Login />} path="/login" />
                    <Route element={<Register/> } path="/register" />
                    <Route element={<ProtectedRoutes />}>
                        <Route element={<WeekPlanner />} path="/planner" />
                    </Route>
            </Routes>
            </BrowserRouter>
        </TokenContext.Provider>
    );
}

export default App;