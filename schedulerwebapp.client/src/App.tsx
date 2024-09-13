import Login from './Login';
import WeekPlanner from './WeekPlanner'
import { BrowserRouter, Routes, Route } from "react-router-dom"
import ProtectedRoutes from "./Tools/ProtectedRoutes"
import Register from './Register';
import { createContext, useState } from 'react';
import ITokenContext from './interfaces/TokenContext';
import { useEffect } from 'react';

export const TokenContext = createContext<ITokenContext>({})
function App() {
    const [jwt, setJwt] = useState(null)
    const tokenContextValue: ITokenContext = { jwt, setJwt }

    useEffect(() => {
        console.log(tokenContextValue.jwt)
    },
    [tokenContextValue.jwt])

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