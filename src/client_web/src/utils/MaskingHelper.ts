export function CardIDMaskingBlind(cardID: string): string {
  const part1 = cardID.slice(0, 1) // 1st digit
  const part2 = cardID.slice(1, 5) // 2nd to 5th digit
  const part3 = cardID.slice(5, 10) // 6th to 10th digit (keep these)
  const part4 = cardID.slice(10, 12) // 11th and 12th digit
  const part5 = cardID.slice(12) // Last digit

  return `${part1}-${'xxxx'}-${'xxxxx'}-${part4}-${part5}`
}
export function CardIDMasking(cardID: string): string {
  const part1 = cardID?.slice(0, 1)
  const part2 = cardID?.slice(1, 5)
  const part3 = cardID?.slice(5, 10)
  const part4 = cardID?.slice(10, 12)
  const part5 = cardID?.slice(12)
  return `${part1}-${part2}-${part3}-${part4}-${part5}`
}
