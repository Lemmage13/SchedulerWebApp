import Login from './Login';
import WeekPlanner from './WeekPlanner'
import { BrowserRouter, Routes, Route } from "react-router-dom"
import ProtectedRoutes from "./Tools/ProtectedRoutes"
import Register from './Register';
function App() {

    return (
        <BrowserRouter>
            <Routes>
                <Route element={<Login />} path="/login" />
                <Route element={<Register/> } path="/register" />
                <Route element={<ProtectedRoutes />}>
                    <Route element={<WeekPlanner />} path="/planner" />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;