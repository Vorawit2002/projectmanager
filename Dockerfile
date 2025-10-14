# ----------------------------
# 🧱 Stage 1: Build the application
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# ติดตั้ง libgdiplus สำหรับ System.Drawing
RUN apt-get update && apt-get install -y libgdiplus

# ตั้ง working directory
WORKDIR /src

# คัดลอกไฟล์โปรเจกต์ (เพื่อใช้ cache restore ได้)
COPY ["ProjectManagement.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY [".editorconfig", "."]

# คัดลอก .csproj ทั้งหมด
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["tests/Application.FunctionalTests/Application.FunctionalTests.csproj", "tests/Application.FunctionalTests/"]
COPY ["tests/Application.UnitTests/Application.UnitTests.csproj", "tests/Application.UnitTests/"]
COPY ["tests/Domain.UnitTests/Domain.UnitTests.csproj", "tests/Domain.UnitTests/"]
COPY ["tests/Infrastructure.IntegrationTests/Infrastructure.IntegrationTests.csproj", "tests/Infrastructure.IntegrationTests/"]

# Restore packages
RUN dotnet restore

# คัดลอกโค้ดทั้งหมด (หลังจาก restore แล้วเพื่อใช้ cache ได้)
COPY . .

# Build solution
RUN dotnet build "src/Web/Web.csproj" -c Release -o /app/build

# Publish แบบไม่ใช้ AppHost (เพื่อลดขนาด)
RUN dotnet publish "src/Web/Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ----------------------------
# 🚀 Stage 2: Run the application
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# ติดตั้ง libgdiplus สำหรับ image/font rendering
RUN apt-get update && apt-get install -y libgdiplus

# ทำ font cache (ถ้ามีการ mount font เข้า container)
WORKDIR /fonts/
RUN fc-cache -f

# App directory
WORKDIR /app

# คัดลอกไฟล์ publish จาก stage ก่อนหน้า
COPY --from=build /app/publish .

# เปิดพอร์ต 80
EXPOSE 80

# จุดเริ่มต้นการรันแอป
ENTRYPOINT ["dotnet", "projectmanagement.Web.dll"]
