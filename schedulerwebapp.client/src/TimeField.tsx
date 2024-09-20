import { MenuItem, Select, SelectChangeEvent, Stack, Typography } from "@mui/material";
import { useState } from "react";

interface timeFieldProps {
    updateTime: (hr: string, mn: string) => void
}
function range(start: number, end: number, increment: number) {
    const arr = [];
    for (let n = start; n < end; n += increment) {
        arr.push(n)
    }
    return arr
}
function twoDigitNum(n: number | string) {
    let num: string = n.toString()
    if (num.length < 2) {
        num = "0" + num
    }
    return num
}
function TimeField(props: timeFieldProps) {
    const {updateTime} = props

    const minutes: Array<number> = range(0, 60, 5)
    const hours: Array<number> = range(0, 24, 1)

    const [numHour, setNumHour] = useState("00")
    const [numMin, setNumMin] = useState("00")

    function handleHourChange(event: SelectChangeEvent) {
        setNumHour(event.target.value)
        updateTime(event.target.value, numMin)
    }
    function handleMinChange(event: SelectChangeEvent) {
        setNumMin(event.target.value)
        updateTime(numHour, event.target.value)
    }

    return (
        <Stack direction="row">
            <Select
                id="hour"
                name="hour"
                label="Hour"
                size="small"
                value={twoDigitNum(numHour)}
                onChange={handleHourChange }
            >
                {
                    hours.map((h) => {
                        return <MenuItem value={twoDigitNum(h)}>{twoDigitNum(h) }</MenuItem>
                    })
                }
            </Select>
            <Typography> : </Typography>
            <Select
                id="min"
                name="minute"
                label="Minute"
                size="small"
                value={twoDigitNum(numMin)}
                onChange={handleMinChange }
            >
                {
                    minutes.map((m) => {
                        return <MenuItem value={twoDigitNum(m)}>{twoDigitNum(m)}</MenuItem>
                    })
                }
            </Select>
        </Stack>
    )
}
export default TimeField