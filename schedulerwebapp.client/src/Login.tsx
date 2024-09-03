import { Button, Card, CardActions, CardHeader, Container, TextField } from "@mui/material"

function Login() {
    return (
        <Container>
            <Card>
                <CardHeader title="Log In" />
                <form>
                    <TextField id="userName" label="User Name" />
                    <TextField id="password" label="Password" />
                    <CardActions>
                        <Button size="small">Submit</Button>
                    </CardActions>
                </form>
            </Card>
        </Container>
    )
}
export default Login