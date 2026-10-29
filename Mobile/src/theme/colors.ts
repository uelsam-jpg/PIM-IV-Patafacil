/**
 * Paleta de cores do PataFácil — alinhada com o Frontend Web (style.css)
 * para manter a identidade visual da marca nos dois canais.
 */
export const colors = {
  primary: "#20b2aa",        // Verde Pet
  primaryDark: "#179c95",
  primaryLight: "#e0f5f4",
  secondary: "#ff7f50",      // Laranja Pet
  secondaryDark: "#fa6c38",
  secondaryLight: "#ffe9e0",

  background: "#f5f7f8",
  card: "#ffffff",
  text: "#1f2933",
  textMuted: "#6b7280",
  border: "#e5e7eb",

  success: "#2e7d32",
  warning: "#f5a623",
  danger: "#e74c3c",
  info: "#20b2aa",

  white: "#ffffff",
  black: "#000000",
} as const;

export type AppColors = typeof colors;
