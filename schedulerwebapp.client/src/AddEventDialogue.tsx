import { Button, Dialog, DialogContent, DialogTitle, MenuItem, Select, Stack, TextField, Typography } from "@mui/material"
import TimeField from "./TimeField"
import { SetStateAction, useState } from "react";
import useAxiosAuth from "./Hooks/UseAxiosAuth";

interface AddEventDialogueProps {
    open: boolean;
    date: Date;
    onClose: () => void;
}
function AddEventDialogue(props: AddEventDialogueProps) {
    const { open, date, onClose } = props;
    const [time, setTime] = useState("00:00:00")
    const [name, setName] = useState("")

    const axiosAuth = useAxiosAuth()

    function updateTime(hr: string, mn: string): void {
        setTime(`${hr}:${mn}:00`)
    }
    function updateName(e: { target: { value: SetStateAction<string>; }; }): void {
        setName(e.target.value)
    }
    async function addRegEvent() {
        try {
            console.log(name)
            console.log(time)
            console.log(date.getDay())
            const response = await axiosAuth.post("https://localhost:54249/api/Events/RegEvents", {
                name: name,
                time: time,
                weekday: date.getDay()
            })
            console.log(response)
        }
        catch (e) {
            console.error(e)
        }
        onClose()
    }
    return (
        <Dialog onClose={onClose} open={open}>
        <DialogTitle>Add Event</DialogTitle>
            <DialogContent>
                <Stack justifyContent="center">
                    <Typography>{ "On: " + date.toDateString() }</Typography>
                    <Select
                        id="eventType"
                        name="eventType"
                        label="Type"
                    >
                        <MenuItem value="regEvent">Regular Event</MenuItem>
                        <MenuItem value="nrReschedule">Reschedule</MenuItem>
                        <MenuItem value="nrUnique">Unique Event</MenuItem>
                    </Select>
                    <TextField
                        id="eventName"
                        name="eventName"
                        label="Event Name"
                        onChange={ updateName }
                    >
                    </TextField>
                    <TimeField updateTime={updateTime}></TimeField>
                    <Button onClick={addRegEvent}>Add Event</Button>
                </Stack>
            </DialogContent>
        </Dialog>
    )
}
export default AddEventDialogue