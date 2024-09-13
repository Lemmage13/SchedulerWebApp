import { Dispatch } from "react";

interface ITokenContext {
    jwt?: string | null,
    setJwt?: Dispatch<React.SetStateAction<string>> | Dispatch<React.SetStateAction<null>>
}
export default ITokenContext