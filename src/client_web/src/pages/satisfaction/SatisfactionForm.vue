<template>
  <v-container>
    <v-card class="pa-4">
      <v-card-title class="text-h5 text-center mb-4">
        แบบประเมินความพึงพอใจต่อระบบ<br>
        (System Satisfaction Assessment)
      </v-card-title>
      
      <v-card-subtitle class="text-center mb-6">
        ระบบจัดการโครงการ (Project Management System)
      </v-card-subtitle>

      <v-card-text>
        <p class="mb-4">
          <strong>คำชี้แจง:</strong> แบบประเมินนี้จัดทำขึ้นเพื่อสอบถามความคิดเห็นของผู้ใช้งานที่มีต่อระบบจัดการโครงการ
          เพื่อนำผลการประเมินไปปรับปรุงและพัฒนาระบบให้มีประสิทธิภาพดียิ่งขึ้น
          โปรดระบุระดับความพึงพอใจของท่านในแต่ละหัวข้อ โดยกำหนดค่าคะแนนดังนี้:
        </p>
        <div class="d-flex justify-center mb-6">
          <v-chip-group>
            <v-chip>5 = มากที่สุด</v-chip>
            <v-chip>4 = มาก</v-chip>
            <v-chip>3 = ปานกลาง</v-chip>
            <v-chip>2 = น้อย</v-chip>
            <v-chip>1 = น้อยที่สุด</v-chip>
          </v-chip-group>
        </div>

        <v-form ref="form" v-model="valid">
          
          <!-- Section 1: Functional Requirement Test -->
          <div v-for="(section, sIndex) in sections" :key="sIndex" class="mb-8">
            <h3 class="text-h6 font-weight-bold mb-4 primary--text">{{ section.title }}</h3>
            
            <v-table>
              <thead>
                <tr>
                  <th class="text-left" style="width: 50%;">รายการประเมิน</th>
                  <th class="text-center" v-for="score in 5" :key="score">{{ 6 - score }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(question, qIndex) in section.questions" :key="qIndex">
                  <td class="py-2">{{ qIndex + 1 }}. {{ question.text }}</td>
                  <td class="text-center" v-for="score in 5" :key="score">
                    <v-radio-group v-model="question.score" hide-details class="justify-center d-flex mt-0">
                      <v-radio :value="6 - score"></v-radio>
                    </v-radio-group>
                  </td>
                </tr>
              </tbody>
            </v-table>
            
            <v-textarea
              v-if="section.hasComment"
              v-model="section.comment"
              :label="`ความคิดเห็นและข้อเสนอแนะ ${section.title}`"
              rows="3"
              outlined
              class="mt-4"
            ></v-textarea>
          </div>

          <v-btn color="primary" block large @click="submit" :loading="loading">
            บันทึกแบบประเมิน
          </v-btn>

        </v-form>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

interface Question {
  text: string;
  score: number | null;
}

interface Section {
  title: string;
  questions: Question[];
  hasComment: boolean;
  comment: string;
}

export default defineComponent({
  name: 'SatisfactionForm',
  data() {
    return {
      valid: false,
      loading: false,
      sections: [
        {
          title: 'ด้านฟังก์ชันความต้องการของระบบ (Functional Requirement Test)',
          hasComment: true,
          comment: '',
          questions: [
            { text: 'ความสามารถของระบบในการจัดการข้อมูล (เพิ่ม ลบ แก้ไข) ข้อมูลผู้ใช้และโครงการ', score: null },
            { text: 'ความสามารถของระบบในการแก้ไขรหัสผ่าน', score: null },
            { text: 'ความสามารถในการแสดงรายละเอียดข้อมูลของโครงการและงาน (Task)', score: null },
            { text: 'ความสามารถของระบบในการค้นหาข้อมูลโครงการและงาน', score: null },
            { text: 'ความสามารถของระบบในการแสดงภาพรวม (Dashboard) และสถานะโครงการ', score: null },
            { text: 'ความสามารถในการใช้งานกระดานแสดงความคิดเห็น/บันทึกกิจกรรมในโครงการ', score: null },
            { text: 'ความสามารถของระบบในการออกรายงานสรุปโครงการ', score: null },
          ] as Question[]
        },
        {
          title: 'ด้านฟังก์ชันความถูกต้องของระบบ (Functional Test)',
          hasComment: true,
          comment: '',
          questions: [
            { text: 'ความถูกต้องในการจัดการข้อมูล (เพิ่ม ลบ แก้ไข) ข้อมูลต่างๆ ในระบบ', score: null },
            { text: 'ความถูกต้องในการทำงานของฟังก์ชันเปลี่ยนรหัสผ่าน', score: null },
            { text: 'ผลลัพธ์ในการค้นหาข้อมูลแสดงได้อย่างถูกต้องและรวดเร็ว', score: null },
            { text: 'ความถูกต้องของสถานะงานและการคำนวณความคืบหน้าโครงการ', score: null },
            { text: 'ความถูกต้องของระบบในการออกรายงาน', score: null },
            { text: 'ความถูกต้องในการทำงานของระบบในภาพรวม', score: null },
          ] as Question[]
        },
        {
          title: 'ด้านการใช้งาน (Usability Test)',
          hasComment: true,
          comment: '',
          questions: [
            { text: 'ความง่ายดายในการเรียนรู้และใช้งานระบบ', score: null },
            { text: 'ความถูกต้องสมบูรณ์ของผลลัพธ์ที่แสดง', score: null },
            { text: 'ความชัดเจนของภาพกราฟิกและไอคอนที่แสดงบนหน้าจอ', score: null },
            { text: 'ความชัดเจนของข้อความและคำอธิบายต่างๆ', score: null },
            { text: 'ความเหมาะสมในการใช้สี ตัวอักษร และการจัดวางรูปแบบ (Layout)', score: null },
            { text: 'ความสะดวกในการเข้าถึงเมนูต่างๆ', score: null },
            { text: 'ปริมาณข้อมูลที่แสดงในแต่ละหน้าจอมีความเหมาะสม', score: null },
            { text: 'ความสม่ำเสมอของรูปแบบการแสดงผลในทุกหน้าจอ', score: null },
            { text: 'ความพึงพอใจต่อภาพลักษณ์โดยรวมของระบบ', score: null },
          ] as Question[]
        },
        {
          title: 'ด้านสิทธิ์การเข้าใช้งานและความปลอดภัย (Security Test)',
          hasComment: true,
          comment: '',
          questions: [
            { text: 'ระบบมีการยืนยันตัวตน (Login) ที่ปลอดภัยและใช้งานได้จริง', score: null },
            { text: 'การกำหนดสิทธิ์การเข้าถึงข้อมูลของสมาชิกแต่ละระดับ (Role) มีความเหมาะสม', score: null },
            { text: 'ระบบมีการป้องกันการเข้าถึงข้อมูลจากผู้ไม่มีสิทธิ์', score: null },
            { text: 'ระบบมีการแจ้งเตือนเมื่อเกิดข้อผิดพลาดหรือกรอกข้อมูลไม่ครบถ้วน', score: null },
            { text: 'ความเชื่อมั่นในระบบรักษาความปลอดภัยของข้อมูล', score: null },
          ] as Question[]
        }
      ] as Section[]
    }
  },
  methods: {
    submit() {
      // Validate that all questions are answered
      let allAnswered = true;
      for (const section of this.sections) {
        for (const q of section.questions) {
          if (q.score === null) {
            allAnswered = false;
            break;
          }
        }
      }

      if (!allAnswered) {
        alert('กรุณาประเมินความพึงพอใจให้ครบทุกข้อ');
        return;
      }

      this.loading = true;
      // Simulate API call
      setTimeout(() => {
        console.log('Assessment Submitted:', this.sections);
        this.loading = false;
        alert('ขอบคุณสำหรับการประเมิน (ข้อมูลถูกบันทึกใน Console)');
        // Reset form or redirect
      }, 1000);
    }
  }
});
</script>

<style scoped>
.v-data-table {
  background-color: transparent !important;
}
</style>
