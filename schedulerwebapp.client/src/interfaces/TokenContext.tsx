import { Dispatch } from "react";

interface ITokenContext {
    token?: string | null,
    setToken?: Dispatch<React.SetStateAction<string>> | Dispatch<React.SetStateAction<null>>
}
export default ITokenContext