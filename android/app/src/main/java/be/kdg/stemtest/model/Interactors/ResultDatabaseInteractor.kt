package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.db.StemTestDb
import be.kdg.stemtest.model.entity.*
import io.reactivex.Maybe
import io.reactivex.Single
import javax.inject.Inject

class ResultDatabaseInteractor @Inject constructor(private val stemTestDb: StemTestDb) {

    fun savePartyResult(result:String){
        stemTestDb.partyResultDao().insert(PartyGameResult(0,result))
    }

    fun saveDebateResult(result:List<DebateGameResult>){
        stemTestDb.debateGameDao().insertAllResults(result)
    }

    fun getPartyGameResult(): Maybe<String>{
        return stemTestDb.partyResultDao().getResult()
            .map { s -> s.score }
    }

    fun getAnswers(): Single<List<StudentAnswer>> {
        return stemTestDb.answerDao().getAllStudentAnswers()
    }

    fun getPartyAnswers(): Single<List<PartyAnswer>> {
        return stemTestDb.partyAnswerDao().getAllPartyAnswers()
    }

    fun saveAntwoord(antwoord: List<PartyAnswer>){

        stemTestDb.partyAnswerDao().saveAllAnswers(antwoord)
    }

    fun getDebateResult(): Maybe<List<DebateGameResult>> {
        return stemTestDb.debateGameDao().getResult()
    }

    fun saveCPGameCorrectAnswers(answerOptions:List<AnswerOption>) {
        stemTestDb.answerOptionDao().updateAllAnswerOptions(answerOptions)
    }

    fun getCorrectAnswers(): Single<List<AnswerOption>> {
        return stemTestDb.answerOptionDao().getAllCorrectAnswerOptions()
    }

    fun getCustomDebateGameResult(index: Int): Maybe<CustomDebateGameResult> {

        return stemTestDb.customDebateGameDao().getCustomDebateGameResult(index)

    }

    fun saveCDgResults(results: List<CustomDebateGameResult>) {
        stemTestDb.customDebateGameDao().saveAllResults(results)
    }
}