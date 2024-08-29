import ICancellation from "./Cancellation";
import INREvent from "./NREvent";
import INRReply from "./NREventReply";
import IRegEvent from "./RegEvent";

interface IEventContainer {
    event: IRegEvent | INREvent;
    cancellation?: ICancellation;
    reply?: INRReply;
}
export default IEventContainer;