import { Button, Card, CardActions, CardHeader, Container, Stack, TextField } from "@mui/material"

function Register() {
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
                    <TextField size="small" id="userName" label="User Name" />
                    <TextField size="small" type="password" id="password" label="Password" />
                    <TextField size="small" type="password" id="passwordConfirmation" label="Confirm Password"/>
                    <CardActions sx={{justifyContent: "center"} }>
                        <Button
                            sx={{width: 1} }
                            size="small">Submit</Button>
                    </CardActions>
                </Stack>
            </Card>
        </Container>
    )
}

export default Register