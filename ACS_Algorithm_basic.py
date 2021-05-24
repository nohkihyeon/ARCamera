import datetime
import time
import FactorySimulatorInterface
import DatabaseInterface

# 전역 변수
server_ip_address = '127.0.0.1'
server_port_number = 6340
number_of_AGV = 6
current_job_count = 0
simulation_loop_time = 1

# function name : get_target_position
# parameters : AGV_number(제어할 AGV 번호), job_id(현재 이송 예정인 제품의 S/N)
# return : target_position(AGV의 목적지)
def get_target_position(AGV_number, job_id):    
    target_position_list = ['HOME,O', 'F-A0,I', 'F-B0,I', 'F-C1,I', 'F-D0,I', 'F-E0,I', 'STRG,I']
    target_position = str()
    current_product_type = str()
    
    # 이송할 제품의 product_type을 확인

    # 목적지 계산

            # product_type을 찾은 경우

            # proudct_type_list의 특정 인덱스를 바탕으로 AGV의 목적지를 계산
            # AGV00 인 경우

            # AGV01 이상인 경우

        # index 함수를 통해 product_type을 찾지 못한 경우

    return(target_position)

# function name : start_program
# parameters : none
# return : none
def start_program():
    DatabaseInterface.init_db("SmartFactory.db")
    DatabaseInterface.get_db_data()
    FactorySimulator.init_socket(server_ip_address,server_port_number)
    FactorySimulator.set_number_of_AGV(number_of_AGV)


# function name : exit_program
# parameters : none
# return : none
def exit_program():
    DatabaseInterface.close()_db
    DatabaseInterface.get_db_data()



# main function  
start_program()
reference_position = ['HOME,O', 'F-A0,O', 'F-B0,O', 'F-C1,O', 'F-D0,O', 'F-E0,O']    

while True:
    # 현재 시뮬레이션 시간 기록
    current_simulation_time = time.time()

    # 입력 문자열에 따라 Step 별로 동작
    input_string = input("command : ").upper()
    if input_string == 'QUIT':
        break

    elif input_string == 'A':
        # AGV 상태 저장
        agv_status_list = FactorySimulatorInterface.get_current_states("AGV_STAT").split(',')
        # AGV 상태 출력
        print("AGV\tStatus\tPos1\tPos2\tSOC\tsn")
        for i in range(number_of_AGV):
            for j in agv_status_list[i * 6 + 1:i * 6 + 7]:
                print(j, '\t', end="")
            print()
        print()

        # Facility 상태 저장
        facility_status_list = FactorySimulatorInterface.get_current_states("FCT_STAT").split(',')
        # Facility 상태 출력
        print("설비\t상태\t입상태\t입AGV\t입예약\t출상태\t출AGV\t출예약\t입버퍼\t제품수\tjobid\t출버퍼\t제품수\tjobid\t내구도")
        for i in range(9):
            for j in facility_status_list[i * 15 + 1:i * 15 + 16]:
                print(j, '\t', end="")
            print()
        print()

        # 6개의 AGV를 제어하는 스케줄링 알고리즘 구현
        for i in range(number_of_AGV):
            # 알고리즘에 사용하는 변수 정의
            # job 관련
            # 이송할 제품이 없는 경우의 job_id : '000000'
            none_job_id = '000000'
            # 전제 job 크기
            total_job_size = len(DatabaseInterface.g_job_list)

            # AGV 관련
            # agv_name 예: 'AGV01', 'AGV02', ..., 'AGV06'
            agv_name = 'AGV' + str(i + 1).zfill(2)
            # agv_status 예:'I', 'M', 'C'
            agv_status = str(agv_status_list[i * 6 + 2])
            # agv_current_position(현재 위치) 예 : 'HOME,O', 'F-B0-I'
            agv_current_position = str(agv_status_list[i * 6 + 3] + ',' + agv_status_list[i * 6 + 4])
            # reference_position(기준 위치) 예: 'HOME,O', 'F-A0,O', 'F-B0,O', 'F-C1,O', 'F-D0,O', 'F-E0,O'
            agv_reference_position = str(reference_position[i])

            # 설비 관련 (F-C2, F-C3 검색 제외)
            # 현재 AGV의 기준 위치에 있는 facility의 output buffer에 쌓인 제품 계산
            # facility_current_output_buffer 예: '00', '01', '02', '03'
            facility_current_output_buffer = str(facility_status_list[(i + 2 if i > 3 else i) * 15 + 13])

            # AGV 이동 명령을 위한 변수
            command = ''

            # AGV의 상태가 Idle 인 경우만 제어

                # AGV가 기준 위치에 없는 경우 기준 위치로 이동 명령

                    # print('Reset' + '\t' + agv_name + '\t' + agv_reference_pos)
                    # command 예 : 'AGV02,000000,F-A0,O'

                # AGV가 기준 위치에 있는 경우

                    # AGV가 AGV01 이고, 이송할 제품이 존재하는 경우

                        # print('Move' + '\t' + agv_name + '\t' + agv_target_pos)
                        # command 예 : 'AGV01,A00001,F-A0,I'

                    # AGV가 AGV01 이 아닌 경우

                        # 기준 위치의 출력 buffer에 이송할 제품이 존재하면 제품을 목적지의 입력 buffer로 이송

                            # print('Move' + '\t' + agv_name + '\t' + agv_target_pos)
                            # command 예 : 'AGV02,A00001,F-B0,I'

        # 시뮬레이션 시간 진행 및 동기화 (시뮬레이션 time step = loop_time)

exit_program()
