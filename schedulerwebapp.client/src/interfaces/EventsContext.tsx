import { Dispatch } from "react";
import ICancellation from "./Cancellation";
import INREvent from "./NREvent";
import IRegEvent from "./RegEvent";
import INRReply from "./NREventReply";

interface IEventsContext {
    regEvents: Array<IRegEvent>
    setRegEvents: Dispatch<React.SetStateAction<IRegEvent[]>>

    nrEvents: Array<INREvent>
    setNREvents: Dispatch<React.SetStateAction<INREvent[]>>

    cancellations: Array<ICancellation>
    setCancellations: Dispatch<React.SetStateAction<ICancellation[]>>

    nreReplies: INRReply[]

    updateEvents: () => Promise<void>
}
export default IEventsContext