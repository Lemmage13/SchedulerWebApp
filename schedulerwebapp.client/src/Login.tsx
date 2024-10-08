import { Box, Button, Card, CardActions, CardHeader, Stack, TextField } from "@mui/material"
import { SetStateAction, useContext } from "react"
import { TokenContext } from "./App"
import {server } from "./Hooks/UseAxios"
import { useState } from "react"
import { useNavigate } from "react-router-dom"

function Login() {
    const tokenContext = useContext(TokenContext)
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")

    const nav = useNavigate()

    async function handleLogin() {
        try {
            const response = await server.post("https://localhost:54249/api/Auth/login",
                {
                    email: email,
                    password: password
                })
            console.log(response.data)
            tokenContext.setJwt ? tokenContext.setJwt(response.data.token) : console.error("SET TOKEN UNDEFINED")
            nav("/planner")
        }
        catch (e) {
            console.error(e)
        }
    }
    function handleEmailChange(e: { target: { value: SetStateAction<string> } }): void {
        setEmail(e.target.value)
    }
    function handlePasswordChange(e: { target: { value: SetStateAction<string> } }): void {
        setPassword(e.target.value)
    }
    return (
        <Box
            sx={[{ display: "flex" },
                { justifyContent: "center" },
                { alignContent: "center" },
                { alignItems: "center" },
                { minHeight: "100vh" }]}>
            <Card sx={[{ px: 8 }, { py: 4 }]}>
                <Stack
                    spacing={2}>
                    <CardHeader
                        title="Log in" />
                    <TextField size="small" id="email" label="Email" value={email} onChange={handleEmailChange} />
                    <TextField size="small" type="password" id="password" label="Password" value={password} onChange={handlePasswordChange} />
                    <CardActions sx={{ justifyContent: "center" }}>
                        <Button
                            sx={{ width: 1 }}
                            size="small"
                            onClick={handleLogin }>Submit</Button>
                    </CardActions>
                </Stack>
            </Card>
        </Box>
    )
}
export default Login