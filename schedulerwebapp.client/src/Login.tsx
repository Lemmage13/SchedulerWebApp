import { Box, Button, Card, CardActions, CardHeader, Stack, TextField } from "@mui/material"

function Login() {
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
                    <TextField size="small" id="userName" label="User Name" />
                    <TextField size="small" type="password" id="password" label="Password" />
                    <CardActions sx={{ justifyContent: "center" }}>
                        <Button
                            sx={{ width: 1 }}
                            size="small">Submit</Button>
                    </CardActions>
                </Stack>
            </Card>
        </Box>
    )
}
export default Login