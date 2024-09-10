import { Button, Card, CardActions, CardHeader, Container, Stack, TextField, Typography } from "@mui/material"
import { SetStateAction, useContext } from "react"
import { TokenContext } from "./App"
import { useState } from "react";
import { useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function Register() {
    const tokenContext = useContext(TokenContext);
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [pw1, setPw1] = useState("");
    const [pw2, setPw2] = useState("");
    const [pwSame, setPwSame] = useState(true)

    const nav = useNavigate()

    async function handleRegister():Promise<void> {
        try {
            const response = await axios.post("https://localhost:54249/api/Auth/register",
                {
                    name: username,
                    email: email,
                    password: pw1
                })
            console.log(response.data)
            tokenContext.setToken ? tokenContext.setToken(response.data.token) : console.error("SET TOKEN UNDEFINED")
            nav("/planner")
        }
        catch (e) {
            console.error(e)
        }
    }


    function handleUsernameChange(e: { target: { value: SetStateAction<string> } }): void {
        setUsername(e.target.value)
    }
    function handleEmailChange(e: { target: { value: SetStateAction<string> } }): void {
        setEmail(e.target.value)
    }
    function handlePw1Change(e: { target: { value: SetStateAction<string> } }): void {
        setPw1(e.target.value)
    }
    function handlePw2Change(e: { target: { value: SetStateAction<string> } }): void {
        setPw2(e.target.value)
    }

    useEffect(() => {
        if (pw1 == pw2) {
            setPwSame(true)
        }
        else {setPwSame(false) }
    }, [pw1, pw2])

    return (
        <Container
            sx={[{ display: "flex" },
            { justifyContent: "center" },
            { alignContent: "center" },
            { alignItems: "center" },
            { minHeight: "100vh" }]}>
            <Card sx={[{ px: 8 }, {py: 4}] }>
                <Stack
                    spacing={2}>
                    <CardHeader
                        title="Register" />
                    <TextField size="small" id="userName" label="User Name" onChange={handleUsernameChange} />
                    <TextField size="small" id="email" label="Email" onChange={handleEmailChange} />
                    <TextField size="small" type="password" id="password" label="Password" onChange={handlePw1Change} />
                    <TextField size="small" type="password" id="passwordConfirmation" label="Confirm Password" onChange={handlePw2Change} />
                    {!pwSame && <Typography>Passwords must be identical</Typography>}
                    <CardActions sx={{justifyContent: "center"} }>
                        <Button
                            sx={{ width: 1 }}
                            size="small"
                            disabled={!pwSame}
                            onClick={handleRegister}>Submit</Button>
                    </CardActions>
                </Stack>
            </Card>
        </Container>
    )
}

export default Register