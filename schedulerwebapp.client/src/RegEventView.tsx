import {useContext} from "react";
import IEventContainer from "./interfaces/EventContainer";
import IMenuMethod from "./interfaces/MenuMethod";
import BasicMenu from "./BasicMenu";
import Card from "@mui/material/Card";
import CardHeader from "@mui/material/CardHeader";
import CardActions from "@mui/material/CardActions";
import Button from "@mui/material/Button";
import IEventsContext from "./interfaces/EventsContext.tsx"
import { EventsContext } from "./WeekPlanner.tsx"
import useAxiosAuth from "./Hooks/UseAxiosAuth.tsx";

interface props {
    eventContainer: IEventContainer;
    disabled: boolean;
    day: Date;
}
function RegEventView({ eventContainer, disabled, day }: props) {
    const eventsContext: IEventsContext | null = useContext(EventsContext)
    const axiosAuth = useAxiosAuth()

    const disabledOrCancelled: boolean = disabled || eventContainer.cancellation != undefined
    const eventIsNotCancelled = eventContainer.cancellation == undefined 

    const optionsMenuItems: Array<IMenuMethod> = [
        { name: "Uncancel Event", method: () => { handleUncancelRegEvent() }, disabled: eventIsNotCancelled },
        { name: "Delete Event", method: () => { handleDeleteregEvent() } }
    ]

    async function handleCancelRegEvent(): Promise<void> {
        try {
            const response = await axiosAuth.post("https://localhost:54249/api/Events/Cancellations", {
                regEventId: eventContainer.event.id,
                date: {
                    Year: day.getFullYear(),
                    Month: day.getMonth() + 1,
                    day: day.getDate()
                }
            })
            console.log(response)
        }
        catch (e) {
            console.log(e)
        }
        eventsContext?.updateEvents()
    }
    async function handleUncancelRegEvent(): Promise<void> {
        if (eventContainer.cancellation != undefined) {
            try {
                const response = await axiosAuth.delete(`https://localhost:54249/api/Events/Cancellations/${eventContainer.cancellation.id}`)
                console.log(response)
            }
            catch (e) {
                console.log(e)
            }
        }
        eventsContext?.updateEvents()
    }
    async function handleDeleteregEvent(): Promise<void> {
        try {
            const response = await axiosAuth.delete(`https://localhost:54249/api/Events/RegEvents/${eventContainer.event.id}`)
            console.log(response)
        }
        catch (e) {
            console.error(e)
        }
        eventsContext?.updateEvents()
    }
    return (
        <Card
            sx={disabledOrCancelled ? {opacity: 0.5} : {opacity: 1} }
        >
            <CardHeader
                title={eventContainer.event.name}
                action={<BasicMenu sx={{ opacity: 1 }} items={optionsMenuItems}></BasicMenu>}
                subheader={eventContainer.event.time}
            >
            </CardHeader>
            <CardActions
                sx={[
                    { justifyContent: "center" }
                ]}
            >
                <Button variant="contained" size="small" title="Cancel" disabled={ disabledOrCancelled } onClick={handleCancelRegEvent}>Cancel</Button>
            </CardActions>
        </Card>
    )
}
export default RegEventView