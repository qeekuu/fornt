import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

interface tempSlice {
  value: number
}

const initialState: tempSlice = {
  value: 0,
}

export const counterSlice = createSlice({
  name: 'temp',
  initialState,
  reducers: {
    increment: (state) => {
      state.value += 1
    },
    decrement: (state) => {
      state.value -= 1
    },
    incrementByAmount: (state, action: PayloadAction<number>) => {
      state.value += action.payload
    },
  },
})

export const counterActions = counterSlice.actions

export default counterSlice.reducer